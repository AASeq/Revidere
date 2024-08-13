#!/bin/sh
SCRIPT_PATH="$(realpath "$0")"
SCRIPT_DIR="$(dirname "$SCRIPT_PATH")"

if [ "$#" -gt 1 ]; then
    >&2 echo "Usage: $(basename $SCRIPT_PATH) [clean|build|release]"
    exit 1
fi


make_clean() {
    echo "$(tput setab 5) $(printf '%-10s' 'clean') $(tput setab 13) $(tput el)$(tput sgr0)"

    mkdir -p "$SCRIPT_DIR/bin"
    find "$SCRIPT_DIR/bin" -mindepth 1 -delete

    mkdir -p "$SCRIPT_DIR/build"
    find "$SCRIPT_DIR/build" -mindepth 1 -delete
}

make_build() {
    mkdir -p "$SCRIPT_DIR/build"

    find "$SCRIPT_DIR/src" -type f -name "*.csproj" -print | sort | while IFS= read -r PROJECT; do
        IS_EXE=`cat "$PROJECT" | grep -q '<OutputType>.*Exe</OutputType>' && echo 1 || echo 0`
        if [ "$IS_EXE" != "0" ]; then
            echo "$(tput setab 5) $(printf '%-10s' 'build') $(tput setab 13) $PROJECT $(tput el)$(tput sgr0)"
            echo
            dotnet publish                                             \
                   --configuration Debug                               \
                   --output "$SCRIPT_DIR/build"                        \
                   "$PROJECT"
            echo
        fi
    done
}

make_run() {
    find "$SCRIPT_DIR/src" -type f -name "*.csproj" -print | sort | while IFS= read -r PROJECT; do
        IS_EXE=`cat "$PROJECT" | grep -q '<OutputType>.*Exe</OutputType>' && echo 1 || echo 0`
        if [ "$IS_EXE" != "0" ]; then
            echo "$(tput setab 5) $(printf '%-10s' 'run') $(tput setab 13) $PROJECT $(tput el)$(tput sgr0)"
            echo
            dotnet run                                                 \
                   --project "$PROJECT"
            break;
        fi
    done
}

make_test() {
    mkdir -p "$SCRIPT_DIR/bin"

    find "$SCRIPT_DIR/test" -type f -name "*.csproj" -print | sort | while IFS= read -r PROJECT; do
        echo "$(tput setab 5) $(printf '%-10s' 'test') $(tput setab 13) $PROJECT $(tput el)$(tput sgr0)"
        echo
        dotnet test                                                 \
               -l  "console;verbosity=detailed"                     \
               "$PROJECT"
        echo
    done
}

make_release() {
    mkdir -p "$SCRIPT_DIR/bin"

    find "$SCRIPT_DIR/src" -type f -name "*.csproj" -print | sort | while IFS= read -r PROJECT; do
        IS_EXE=`cat "$PROJECT" | grep -q '<OutputType>.*Exe</OutputType>' && echo 1 || echo 0`
        if [ "$IS_EXE" != "0" ]; then
            echo "$(tput setab 5) $(printf '%-10s' 'release') $(tput setab 13) $PROJECT $(tput el)$(tput sgr0)"
            echo
            dotnet publish                                             \
                   -p:PublishReadyToRun=true -p:PublishSingleFile=true \
                   --self-contained true --use-current-runtime         \
                   --configuration Release                             \
                   --output "$SCRIPT_DIR/bin"                          \
                   "$PROJECT"
            echo
        fi
    done
}

make_docker() {
    mkdir -p "$SCRIPT_DIR/build/docker"

    find "$SCRIPT_DIR/src" -type f -name "*.csproj" -print | sort | while IFS= read -r PROJECT; do
        IS_EXE=`cat "$PROJECT" | grep -q '<OutputType>.*Exe</OutputType>' && echo 1 || echo 0`
        if [ "$IS_EXE" != "0" ]; then
            echo "$(tput setab 5) $(printf '%-10s' 'docker') $(tput setab 13) $PROJECT $(tput el)$(tput sgr0)"
            echo
            dotnet publish                                             \
                   -t:PublishContainer                                 \
                   --no-self-contained                                 \
                   --configuration Release                             \
                   --output "$SCRIPT_DIR/build/docker"                 \
                   "$PROJECT" || exit 1
            echo

            VERSION=`cat "$PROJECT" | grep '<Version>.*</Version>' | sed 's/.*<Version>\(.*\)<\/Version>.*/\1/'`
            if [ "$VERSION" = "0.0.0" ]; then
                echo "$(tput setaf 11)Version not set.$(tput sgr0)"
            fi
            echo

            if [ "$VERSION" != "0.0.0" ]; then
                DOCKER_REPO=`cat $SCRIPT_DIR/.dockerrepo 2>/dev/null | awk '{print $1}'`
                if [ "$DOCKER_REPO" != "" ]; then
                    for TAG in "latest" "$VERSION"; do
                        docker tag revidere:latest $DOCKER_REPO:$TAG
                        docker push $DOCKER_REPO:$TAG
                        echo
                    done
                fi
            fi
        fi
    done
}


ACTION=$1
if [ "$ACTION" = "" ]; then ACTION="all"; fi
if [ "$ACTION" = "all" ]; then ACTION="release"; fi

echo

case $ACTION in
    clean)
        make_clean
    ;;

    build)
        make_build
    ;;

    run)
        make_run
    ;;

    test)
        make_test
    ;;

    release)
        if ! [ -n "$MAKELEVEL" ]; then make_clean; fi
        make_release
    ;;

    docker)
        if ! [ -n "$MAKELEVEL" ]; then make_clean; fi
        make_docker
    ;;

    *)
        >&2 echo "Unknown action $ACTION"
        exit 1
    ;;
esac
