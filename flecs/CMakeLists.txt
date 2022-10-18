cmake_minimum_required(VERSION 3.23)
project(flecs C)

set(CMAKE_C_STANDARD 11)

include_directories(src)

add_executable(flecs
        src/flecs.c
        src/flecs.h
        src/main.c)
