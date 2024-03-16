# Long2Long
C# projects that is convert long text to long text using LLMs


# Long2Long - 長文から長文を生成します

LLMを使っていて今までこんなことで困ったことはありませんか？

- 長文を訳そうとして貼り付けたら、全部一気に実行せずに、continueを押しても途中でバグってしまう
- 長文を同じチャットに貼り付けると、LLMの作業キャパを超えるので別チャンネルで実行して、整理ができない
- 別のLLMとの動作を比較したいがいろんなところに貼るのが面倒
- 本当に長い文を全部翻訳したりするときに、貼る量の管理が難しい
- ブラウザに貼って、待って、ローカルに結果を貼れば全部実行できるが、時間がかかりすぎて面倒

Long2Longは長文から長文をLLMに生成させる、LLM実行に特化した


.NETで作成した、LLMのコマンドライン実行アプリケーションです。以下の機能を含んでいます。

- 長文を指定したトークン数で分割して実行する
- 複数のLLMサービスに並行してリクエストできる
- フェーズ実行（複数のプロンプトを用意して、1個目のプロンプトでできた結果に対して2個目のプロンプトを実行できる）
- ある程度並列で実行するので、全体の処理時間がかなり早い
- エラー時のリトライ機能
 
上記の機能があるので、長文を分割してWebブラウザに切りはりして、結果もまた切りはりしてテキストファイルに戻すなど必要なく、一回実行を押して後からゆっくり結果を読むことができます。

## 使い方


1. **L2LConsole フォルダに移動**
```sh
$ cd src/L2LConsole
```

2. **APIの実行のためのAPIキーを準備する**

各クラウドでLLMを実行するためのAPIキーを準備します。以下の４サイトに対応していますが、実行したいサイトのみの準備で大丈夫です。複数準備したら、複数を並行実行できるので便利です。

   2-1. **Azure OpenAI**

   Azure OpenAI でモデルを作成し、以下の情報を準備します
```
  "AzureOpenAiUrl": "",
  "AzureOpenAiModel": "",
  "AzureOpenAiApiKey": "",
```

   2-1. **Anthropic Claude**

[Anthrorpic API](https://www.anthropic.com/api)サイトで以下の情報を取得します。
```
  "AnthropicModel": "デフォルトは claude-3-opus-20240229",
  "AnthropicApiKey": "",
```

   2-3. **Google Gemini**

[Google AI Studio](https://aistudio.google.com/)サイトで以下の情報を取得します
```
  "GeminiApiKey": ""
  "GeminiModel": "デフォルトはgemini-pro"
```

   2-4. **OpenAI API**

[Open AI API](https://openai.com/blog/openai-api)サイトで以下の情報を取得します
```
  "OpenAiModel": "デフォルトはgpt-4-turbo-preview",
  "OpenAiApiKey": "",
```

3. **.NET 開発環境を作成する**

実行ファイルをリリースすれば開発環境以外で実行することが

4. **設定ファイルを作成する**
