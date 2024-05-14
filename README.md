# md2m3u
Multi Disc Game to .m3u

Scans the current path for game images looking for the one of the following extensions.<br />
".cue"<br />
".iso"<br />
".chd"<br />
".cdi"<br />
".gdi"<br />
".rvz"<br />
".d64"<br />

An .m3u file is created when a multi disc game is detected.  Multi Disc Games can be found with one of the following patterns.<br />
" (Disc [0-9])"<br />
" (Disc[0-9])"<br />
" - DVD-[0-9]"<br />
" - CD[0-9]"<br />
" (Disk [0-9])"<br />
" (Disk [0-9] Side [AB])"<br />
" (Side [AB])"<br />

all - Run with the "all" flag to create .m3u for all detected games.<br />
recursive - Run with the "recursive" to scan for images recursively<br />

Multiple Discs must be in the same folder.<br />

Simply extract the zip in the top level folder to scan and then execute the desired bat file.<br />
