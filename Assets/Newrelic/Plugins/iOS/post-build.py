import os
from sys import argv
from mod_pbxproj import XcodeProject
import shutil

scriptPath = os.path.dirname(argv[0])
fileToAddPath = argv[1]

print("``copying NewRelicAgent.framework to " + fileToAddPath + "/")
shutil.copytree( scriptPath + '/NewRelicAgent.framework/',fileToAddPath + '/NewRelicAgent.framework')

project = XcodeProject.Load(fileToAddPath + '/Unity-iPhone.xcodeproj/project.pbxproj')
project.add_file('System/Library/Frameworks/CoreTelephony.framework', tree='SDKROOT')
project.add_file('System/Library/Frameworks/SystemConfiguration.framework', tree='SDKROOT')
project.add_file('usr/lib/libc++.dylib', tree='SDKROOT')
project.add_file('usr/lib/libz.dylib', tree='SDKROOT')
project.add_file(fileToAddPath + "/NewRelicAgent.framework")

project.apply_mods({'compiler_flags':{'-fno-objc-arc':['NewRelicUnityPlugin.m']}})

if project.modified:
    project.backup()
    project.saveFormat3_2();
