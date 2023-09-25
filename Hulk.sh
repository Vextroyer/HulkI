#This script simplify usage of the Hulk interpreter.
#It compiles the interpreter and run it.

#Function declarations

#Exists command
#Determines if a given program exists. It only receives one parameter, the name of the program.
function Exists(){
    if ! command -v $1 &> /dev/null
    then
        #Doesnt exists
        return 1
    else
        #Exists
        return 0
    fi
}

#Run command
function Run(){
    #echo Run

    local status=0

    if Exists dotnet; then

        echo "Starting interpreter. Press Ctrl+C at any time to stop excecution."
        #Compilation and execution
        dotnet run --project $projectFile

    else
        echo "dotnet command not found, stoping execution."
        exit
    fi
}

#Help command
function Help(){
    echo "Usage: Hulk.sh [parameters]"
    echo "[parameters] is optional. If no parameter is specified then run is used as default."
    echo ""
    echo "Parameters:"
    echo ""
    echo "  clean                       Remove files generated during compilation."
    echo "  help                        Shows this message."
    echo "  run                         Compiles and run the interpreter."
}

#Clean command
#It must erase the following files
#Generated at compilation:               -obj
#                                        -bin
function Clean(){
    # C# compiler generated files
    echo "Removing C# generated files"
    echo ""
    rm -r -f -d -v "bin"
    rm -r -f -d -v "obj"
    echo ""
    echo "Done."
    echo ""
}

#End of function declarations

#Variables

#Get the scripts directory, de https://stackoverflow.com/questions/4774054/reliable-way-for-a-bash-script-to-get-the-full-path-to-itself
#Scripts directory
scriptDirectory="$( cd -- "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"

#The csproj file
projectFile="HulkInterpreter.csproj"

#End of Variables

#Script starts executing

case $# in
    0)
        #No arguments means the program is to be runned.
        Run
    ;;
    
    #A parameter has been passed
    *)
        case $1 in
            "clean")
                Clean
            ;;

            "run")
                #Runs the program
                Run
            ;;
    
            "help")
                Help
            ;;

            *)
                #Unknown command
                echo "Unknown command. Showing help message."
                echo ""
                Help
            ;;
        esac
    ;;

esac

#Script stops executing