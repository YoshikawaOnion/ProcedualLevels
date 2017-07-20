import os;
import sys;

behaviorAssetFile = 'CodeGenerator/XAsset.cs'
assetEditorFile = 'CodeGenerator/XAssetEditor.cs'
behaviorAssetGenPath = '../Assets/Scripts/Models/$name$Asset.cs'
assetEditorGenPath = '../Assets/Scripts/Editor/$name$AssetEditor.cs'

def copy(source, destination, name):
    with open(source, 'r') as f:
        str = f.read()
    str = str.replace('$name$', name)
    genPath = destination.replace('$name$', name)
    with open(genPath, 'w') as f:
        f.write(str)

if __name__ == '__main__':
    name = sys.argv[1]
    copy(behaviorAssetFile, behaviorAssetGenPath, name)
    copy(assetEditorFile, assetEditorGenPath, name)
    print('Files generated.')
