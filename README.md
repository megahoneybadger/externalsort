
## General
This is an open source library which allows to sort big text files using external sorting. 


### How to use
Firstly get the sources:

    git clone https://github.com/megahoneybadger/externalsort
    cd externalsort

Secondly build the sources server and client (you will need .net core v5):

    make rebuild

To generate a new  file:

    dotnet _bin/altium.gen.dll -s 10kb -n MyBigFile.txt
    
 - -s File size (accepts strings like 10kb or 2Gb)
 - -n File name
 
To sort an existing file:

    dotnet _bin/altium.sort.dll -n MyBigFile.txt
    
As a result, the application creates a new file with suffix "[sorted]" at the same folder.
