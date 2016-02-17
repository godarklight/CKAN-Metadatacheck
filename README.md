A metadata checker for CKAN.

Required debian packages:
mono-complete
rsync

Folder structure:
Put a KSP folder containing KSP_linux-$VERSION, and set the version in run.sh. Also create a downloads folder for CKAN to dump its info (it will be symlinked into KSP to preserve the cache).

An example tree output:
ckan.exe
CKAN-Metacheck.exe
downloads
KSP/
KSP/KSP_linux-1.0.5/
Newtonsoft.Json.dll
run.sh


If you are building in monodevelop:
First put ckan.exe inside the CKAN-Metacheck project folder (where the source files are, not the solution file).

the json nuget package will automatically download, hitting build will setup the build directly correctly (with the extra debug symbols however) and you should be able to run ./run.sh. It will ask you to place KSP in the directory.