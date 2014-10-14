#!/bin/bash

echo '******************************************************'
echo '*              Component Build Script                *'
echo '******************************************************'
echo ''


echo 'Deleting old Component Tools...'
rm -rf ./tools/
mkdir ./tools/
cd ./tools/

echo 'Downloading latest Component Tools from Xamarin...'
curl -silent -L https://components.xamarin.com/submit/xpkg > tools.zip

echo 'Extracting downloaded Component Tools...'
unzip -q -o tools.zip 2> /dev/null

echo 'Cleaning up...'
rm tools.zip
cd ..

echo ''

echo 'Building Component...'
mono ./tools/xamarin-component.exe package ./

echo 'Building NuGet package...'
#Gets the newest .xam file (which should be the one we just made!)
#NEWXAM=$(ls -t *.xam | head -1)
#mono ./tools/xamarin-component.exe transform -v $NEWXAM

echo ''
echo 'Cleaning up...'
rm -rf ./tools/

echo ''
echo '******************************************************'
echo '*                        DONE                        *'
echo '******************************************************'

