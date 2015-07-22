@echo off
echo RandomSimple
echo RandomNoDouble
echo RotateCW
echo RotateCCW
echo RotateBoth
echo RotateBothRandom
set /p algo="Enter Algorithm: "
start 2048.exe %algo%
exit