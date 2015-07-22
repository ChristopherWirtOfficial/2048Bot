# 2048Bot
Command-line 2048 game for bruteforce gameplay testing

# How-To
The application is purely command line, and may be run from a shortcut, the provided Run.bat or any other command line interface. The command to run it is

    2048.exe [algorithm]
    
An algorithm may be specified, with the default being RotateCW (because it was the original reason for this project)

Supported algorithms are below. If you want to know exactly what they do, check the source code!

 - RandomSimple
 - RandomNoDouble
 - RotateCW
 - RotateCCW
 - RotateBoth
 - RotateBothRandom

When using Run.bat, the entire algorithm name should be entered, or left blank for default.

Since each instance is single-threaded, each instance will use at most a single logical core. This means you can still effectively run multiple algorithms at once, but it is unoptimized for using a full CPU on a single algorithm. This will likely change in the future. [Fork me on github!]

Each of these will create its own scores.json file in the current directory. These scores are cumulitive for a given test, and save every 10,000 games (Every one or two seconds, roughly). If you start again with the same algorithm, you will start from the most recent save. The application will run until exited, because of this. Please do not try to run multiple instances with the same algorithm specified, because **it does NOT read from the file between saves**, only at the start.

You can find a binary in the Debug folder. 
 1. Yes, I commited binaries
 2. Yes, I didn't even make a release
 3. No, I will not remove them or fix it, get off my back


I hope to eventually have a web portal to upload your json files to, and maybe even something more automatic and cloud-based that will show "real-time" results across all instances of the application. Thank you for using this! You're furthering ... My desires. And maybe science. Mostly my wants, and maybe yours!
