JSUT (Japanese speech corpus of Saruwatari Lab, University of Tokyo) corpus

==== 内容 ==== 
 このコーパスは日本語テキストと読み上げ音声からなります．音声データは48kHzでサンプリングされ，無響室で収録されました．一人の日本語女性話者の音声を収録しました．このコーパスは，10時間 を含み，以下のデータからなります． 
   - basic5000 ... 常用漢字の音読み・訓読みを全てカバー
   - utparaphrase512 ... 文の一部を読み替えたもの
   - onomatopee300 ... 日本語オノマトペ
   - countersuffix26 ... 数詞
   - loanword128 ... 外来語由来の動詞・名詞 (e.g., ググる)
   - voiceactress100 ... 声優統計コーパス (プロ女性声優のフリーコーパス) とのパラ音声
   - travel1000 ... 旅行ドメインのフレーズ
   - precedent130 ... 判例文
   - repeat500 ... 繰り返し発話された音声 (100文 * 5回)

 各ディレクトリには，以下のものが入っています． 
   - /wav/ ... 音声データ
   - /transcript_utf8.txt ... テキストデータ
   - /recording_info.txt ... いつ音声を収録したか (MM/DD/YYYY)  
   - /memo.txt ... アノテータのコメント
   
==== 使い方 ==== 
 テキストデータは，CC-BY-SA 4.0などにてライセンスされております．詳細は，LICENCEファイルをご覧ください．音声データは，以下の場合に限り使用可能です．
   - アカデミック機関での研究
   - 非商用目的の研究（営利団体での研究も含む）
   - 個人での利用（ブログなどを含む）
営利目的の利用を希望される場合，私 (@forthshinji or email) まで連絡して下さい．この音声データの再配布は認められていませんが，あなたのウェブページやブログなどでコーパスの一部（例えば，100文程度）を公開することは可能です．できれば，あなたが論文やブログポスト等の成果を公開した際には，私まで連絡してもらえると助かります．このコーパスの貢献を調査することは，我々にとって非常に有効な情報となります．

==== 作成者 ==== 
 園部 良介 (東京大学) 
 高道 慎之介 (東京大学)
 猿渡 洋 (東京大学)

高道 慎之介 (shinnosuke_takamichi@ipc.i.u-tokyo.ac.jp) が主な責任者です．

==== 商用利用 ====
我々は，皆様の商用利用を歓迎します．下記メールアドレスにご連絡ください．
居石 圭司 (東大TLO): sueishi [_at_mark_] todaitlo.jp
高道 慎之介: shinnosuke_takamichi [_at_mark_] ipc.i.u-tokyo.ac.jp

==== 引用 ====
Ryosuke Sonobe, Shinnosuke Takamichi and Hiroshi Saruwatari, 
"JSUT corpus: free large-scale Japanese speech corpus for end-to-end speech synthesis,"
arXiv preprint, 1711.00354, 2017.
