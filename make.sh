#!/bin/sh
SCRIPT_PATH="$(realpath "$0")"
SCRIPT_DIR="$(dirname "$SCRIPT_PATH")"

if [ "$#" -gt 1 ]; then
    >&2 echo "Usage: $(basename $SCRIPT_PATH) [clean|build|release]"
    exit 1
fi


clean() {
    echo "$(tput setab 5) $(printf '%-10s' 'clean') $(tput setab 13) $(tput el)$(tput sgr0)"

    mkdir -p "$SCRIPT_DIR/bin"
    find "$SCRIPT_DIR/bin" -mindepth 1 -delete

    mkdir -p "$SCRIPT_DIR/build"
    find "$SCRIPT_DIR/build" -mindepth 1 -delete
}

build() {
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

run() {
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

test() {\
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

release() {
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

docker() {
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
                   "$PROJECT"
            echo
        fi
    done
}


ACTION=$1
if [ "$ACTION" = "" ]; then ACTION="all"; fi
if [ "$ACTION" = "all" ]; then ACTION="release"; fi

echo

case $ACTION in
    clean)
        clean
    ;;

    build)
        build
    ;;

    run)
        run
    ;;

    test)
        test
    ;;

    release)
        if ! [ -n "$MAKELEVEL" ]; then clean; fi
        release
    ;;

    docker)
        if ! [ -n "$MAKELEVEL" ]; then clean; fi
        docker
    ;;

    *)
        >&2 echo "Unknown action $ACTION"
        exit 1
    ;;
esac
