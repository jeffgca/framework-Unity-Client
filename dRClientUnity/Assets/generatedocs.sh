#!/bin/bash

UNITY_ASSEMBLY=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll
DIMEROCKER_ASSEMBLY=

gmcs -r:$UNITY_ASSEMBLY -r:Standard\ Assets/dimeRocker\ Custom\ Framework/Assemblies/Procurios.Public.dll Standard\ Assets/dimeRocker\ Custom\ Framework/Scripts/*.cs /doc:Documentation/doc.xml -t:library
mdoc update -i Documentation/doc.xml -o Documentation/original -r $UNITY_ASSEMBLY Standard\ Assets/dimeRocker\ Custom\ Framework/Scripts/dimeRocker.dll
mdoc export-html --template Documentation/doctemplate.xsl -o Documentation Documentation/original
rm Documentation/doc.xml
rm -rf Documentation/original
rm Standard\ Assets/dimeRocker\ Custom\ Framework/Scripts/dimeRocker.dll
