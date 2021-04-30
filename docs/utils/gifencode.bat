@echo off
rem From http://blog.pkh.me/p/21-high-quality-gif-with-ffmpeg.html with thanks.
set PALETTE=%TEMP%\palette.png
set FILTERS=fps=10,scale=320:-1:flags=lanczos

ffmpeg -v warning -i %1 -vf %FILTERS%,palettegen -y %PALETTE%
ffmpeg -v warning -i %1 -i %PALETTE% -lavfi "%FILTERS% [x]; [x][1:v] paletteuse" -loop 4 -y %~n1.gif