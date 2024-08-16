# PreviewTextureMaker
Materialに設定されたShaderを利用してTextureを生成できるEditor拡張

# 動画
https://github.com/user-attachments/assets/2a673ef7-8ef6-4c5f-8e5f-2b49144cf5bb

# 導入方法
* unitypackageをダウンロードして使う

# 使い方
![image](https://github.com/user-attachments/assets/09efb0de-f48c-453f-9d9f-4bdbabb48687)<br>
マテリアルを右クリックし、Create > MaterialPreviewTextureを選択

![image](https://github.com/user-attachments/assets/d0dd8ebf-42d3-49cb-81b6-05b96970b96a)<br>
専用Windowが出ます。

テクスチャサイズ
|||
|--|--|
|Half|512|
|Normal|1024|
|Large|2048|
|Custom|任意|

生成ボタンで生成し、保存します。

また、下のマテリアルのインスペクターでマテリアルのパラメータを設定し、その状態でテクスチャを生成できます。

# 不具合
* _Timeを使用しているマテリアルだとプレビューの画像が常に変わってしまいます。
