import argparse
import tempfile
import os
from shutil import unpack_archive
import stat
import csv

if __name__ == "__main__":
  parser = argparse.ArgumentParser()
  parser.add_argument("-p", default=0,
      help="UnityPackageのパス")
  parser.add_argument("-o", type=str, default=0,
      help="出力ディレクトリのパス")
  parser.add_argument("--nometa", action='store_true', default=False,
      help=".metaファイルをエクスポート時に作成しない")
  parser.add_argument("--csv", action='store_true', default=False,
      help=".csvファイルを作成")
  args = parser.parse_args()

UsePause = False

if args.p == 0:
    UsePause = True
    args.p = input("UnityPackageのパス: ")
    args.o = input("出力ディレクトリのパス (出力しない場合は何も入力しない): ")
    if args.o != "":
        while True:
            yn = input(".metaファイルをエクスポート時に作成しない (y/N): ").lower()
            if yn in ['y', 'ye', 'yes']:
                args.nometa = True
                break
            elif yn in ['n', 'no']:
                args.nometa = False
                break
    elif args.o == "":
        args.o = 0
    while True:
            yn = input(".csvファイルを作成 (y/N): ").lower()
            if yn in ['y', 'ye', 'yes']:
                args.csv = True
                break
            elif yn in ['n', 'no']:
                args.csv = False
                break

temp_directory = tempfile.TemporaryDirectory()
try:
    unpack_archive(filename=args.p, extract_dir=temp_directory.name, format="gztar")
except OSError:
    print("Error: 無効なファイル (UnityPackageですか?)")
    exit()

folders = os.listdir(temp_directory.name)

csvdata = [["ファイルのパス","ファイルのサイズ (MB)","GUID"]]

package_size = os.path.getsize(args.p)
print(f"ロードしたパッケージ:\n[{str(round(package_size/1024/1024, 2)).rjust(7)} MB] %s\n" % (args.p))

print("含まれているファイル:")
for folder in folders:
    asset_size = 0
    folder_path = temp_directory.name + "/" + folder
    files = os.listdir(folder_path)
    with open(folder_path + "/pathname") as f:
        s = f.read()
        try:
            asset_size = os.path.getsize(folder_path + "/asset")
            FileSize = asset_size
            print(f"[{str(round(asset_size/1024/1024, 2)).rjust(7)} MB] %s" % (s))
        except FileNotFoundError:
            pass
    
    csvdata.append([s,asset_size/1024/1024,os.path.split(folder_path)[1]])

if args.o != 0:
    print("\nファイルを\"%s\"に出力中..." % (args.o))
    args.o = args.o + "/" + str(os.path.splitext(os.path.basename(args.p))[0])
    
    if os.path.isdir(args.o) == True:
        while True:
            yn = input("ディレクトリは既に存在します！上書きしますか？ (y/N): ").lower()
            if yn in ['y', 'ye', 'yes']:
                break
            elif yn in ['n', 'no']:
                print("Abort.")
                exit()

    for folder in folders:
        folder_path = temp_directory.name + "/" + folder
        files = os.listdir(folder_path)

        with open(folder_path + "/pathname") as f:
            s = f.read()

        savepath = args.o + "/" + s
        dir = os.path.dirname(savepath)

        if os.path.isdir(dir) == False:
            os.makedirs(dir, exist_ok=True)
            os.chmod(dir, mode = stat.S_IRUSR | stat.S_IWUSR | stat.S_IRGRP | stat.S_IROTH)
        
        try:
            with open(folder_path + "/asset","rb") as bin:
                b = bin.read()
            
            with open(savepath,"wb+") as save:
                save.write(b)
                save.close()
        except FileNotFoundError:
            pass

        if args.nometa == False:
            try:
                with open(folder_path + "/asset.meta","rb") as bin:
                        b = bin.read()
                        metapath = args.o + "/" + os.path.dirname(s) + "/" + os.path.splitext(os.path.basename(s))[0] + ".meta"
                
                with open(metapath,"wb+") as save:
                    save.write(b)
                    save.close()
            except FileNotFoundError:
                print("Error: .metaファイルの生成中にエラーが発生しました")

if args.csv == True:
    
    if args.o == 0: args.o = "."
    resultpath = args.o + "/" + str(os.path.splitext(os.path.basename(args.p))[0]) + "_result.csv"

    print("csvファイルを\"%s\"に出力中..." % os.path.abspath(resultpath))
    with open(resultpath, 'w') as f:
        writer = csv.writer(f)
        writer.writerows(csvdata)

print("完了")

if UsePause:
    os.system('PAUSE')

