# All commands are executed in the make.sh script

.PHONY: all build clean docker release run test
all: release

clean:
	@./make.sh clean

build:
	@./make.sh build

run:
	@./make.sh run

test:
	@./make.sh test

release: clean
	@./make.sh release

docker: clean
	@./make.sh docker
