# -*- coding: utf_8 -*-

# 参考ページ
# PyQt
# https://qiita.com/phyblas/items/d56003904c83938823f2
# 
# Pyinstaller command(activate後のcmd上)
# pyinstaller TempChecker.py --onefile --noconsole
# https://techplay.jp/column/1646
# https://pyinstaller.org/en/stable/
# 出力先
# \TempChecker\TempChecker\dist\TempChecker.exe

# オフラインインストール
# https://qiita.com/FJKei/items/03fbf38eaf6292fd5441

import sys
#from typing import Self
#from PyQt6.QtCore import Qt
#from PyQt6.QtCore import *
#from PyQt6.QtWidgets import QApplication,QWidget,QLineEdit,QLabel, QPushButton, QSplitter

# 「PyQt」のモジュール群をインポートする
from PyQt6.QtCore import *
from PyQt6.QtWidgets import *

import sys
import time
# 「sipモジュール」をインポートする
import sip
# 「datetimeモジュール」をインポートする
import datetime
# 「localeモジュール」をインポートする
import locale


class TempChecker(QWidget):
    def __init__(self):
        # QWidgetクラスを初期化する
        super().__init__()
        # システムロケールを設定する
        # setlocale()を呼び出さなければ、PyQtで日本語を表示できない
        locale.setlocale(locale.LC_ALL, "")
        # PyQtのウィンドウを生成する
        #self.createWindow()
        
        # QTimerを初期化する
        timer = QTimer(self)
        # timeoutシグナルに「getDateTime()メソッド」をスロットとして指定する
        timer.timeout.connect(self.getDateTime) ##周期動作を設定
        # タイマーの動作を開始する
        timer.start()

        self.setWindowTitle('TempChecker') # ウィンドウのタイトル
        self.open_x= 100;
        self.open_y = 100;
        self.window_width= 600;
        self.window_height = 400;
        self.setGeometry(self.open_x,self.open_y,self.window_width,self.window_height) # ウィンドウの位置と大きさ
        self.setStyleSheet('font-family: Kaiti SC; font-size: 20px;')
        css = '''
        background-color: #e7869f;
        color: #635d81;
        font-family: Kaiti SC;
        font-weight: bold;
        font-size: 36px;
        font-style: italic;
        text-decoration: underline;
        '''
        self.setStyleSheet(css)
        
        split_Horizontal = QSplitter(Qt.Orientation.Horizontal,self) # 横で分裂
        split_Vertical = QSplitter(Qt.Orientation.Vertical,self) # 縦で分裂
        

        self.rigth_butoom_label = 'Right_Bottom'
        self.Right_Bottom = QLabel(self.rigth_butoom_label,self)
        
        split_Horizontal.addWidget(self.Right_Bottom)

        split_Vertical.addWidget(QLabel('Left_Top',self))
        split_Horizontal.addWidget(split_Vertical)
        split_Horizontal.addWidget(QLabel('Left_Bottom',self))
        split_Vertical.addWidget(QLabel('Right_Top',self))
        
        
        #self.Right_Bottom.setGeometry(20,20,300,110)
        self.Right_Bottom.setStyleSheet(css)
        
        
        """
        self.grid = QGridLayout()
        self.setLayout(self.grid)
                
        self.grid.addWidget(QLabel('Left_Top',self),0,0)
        self.grid.addWidget(QLabel('Left_Bottom',self),0,0)
        self.grid.addWidget(QLabel('Right_Top',self),0,0)
        self.grid.addWidget(QLabel('Right_Bottom',self),0,0)
        """
        #self.connect(Right_Bottom,QtCore.SIGNAL("textEdited(QString)"),self.write_lbl)
        
        

        #split_Horizontal.setGeometry(10,10,400,120)

        #css = '''background-color: #e7869f;color: #635d81;font-family: Kaiti SC;font-weight: bold;font-size: 36px;font-style: italic;text-decoration: underline;'''
        #self.setStyleSheet(css)
        #botan = QPushButton('第二ボタン',self)
        #css = '''background-color: #fae4cb;border: 3px solid red;text-align: right;padding: 15px;'''
        '''
        
        botan.setStyleSheet(css)
        botan.setGeometry(20,20,250,110)
        self.setStyleSheet('font-family: Kaiti SC; font-size: 20px;')
        self.hako = QLineEdit('編集してみて',self)
        self.hako.setGeometry(10,10,150,30)
        self.hako.textChanged.connect(self.henshuu)
        
        self.shirase = QLabel('編集してみて',self)
        self.shirase.setGeometry(10,50,150,30)
    
    def henshuu(self):
        self.shirase.clear()
        self.shirase.setText(str(self.hako.text()))
        '''
        
        
    # createWindow()メソッド｜PyQtのウィンドウを生成する
    def createWindow(self):

        # ウィンドウのタイトルを設定する
        self.setWindowTitle("PyQtサンプルプログラム")

        # ウィンドウのサイズを設定する
        self.setGeometry(0, 0, 640, 480)

        # ウィンドウを表示する
        self.show()

        # QTimerを初期化する
        timer = QTimer(self)

        # timeoutシグナルに「getDateTime()メソッド」をスロットとして指定する
        timer.timeout.connect(self.getDateTime) ##周期動作を設定

        # タイマーの動作を開始する
        timer.start()

    # getDateTime()メソッド｜ステータスバーに現在日時を表示する
    def getDateTime(self):
        # 現在の日時を取得する
        time = datetime.datetime.today()

        # 取得した日時をフォーマット済み文字列形式に変換する
        #string = time.strftime(u"%Y年%m月%d日 %H時%M分%S秒")
        string = time.strftime(u"%H時%M分%S秒")

        # ステータスバーに現在日時を表示する
        #self.statusBar().showMessage(f"現在時刻：{string}")
        #Right_Bottom = QLabel(f"現在時刻：{string}",self)
        self.Right_Bottom.setText(f"t:{string}") 
        



# Programクラス｜PyQtの処理を実行する
# さまざまな機能を搭載した「QMainWindowクラス」を継承する
class Program(QMainWindow):
    # コンストラクタ
    def __init__(self):

        # QWidgetクラスを初期化する
        super().__init__()
    
        # システムロケールを設定する
        # setlocale()を呼び出さなければ、PyQtで日本語を表示できない
        locale.setlocale(locale.LC_ALL, "")

        # PyQtのウィンドウを生成する
        self.createWindow()

    # createWindow()メソッド｜PyQtのウィンドウを生成する
    def createWindow(self):

        # ウィンドウのタイトルを設定する
        self.setWindowTitle("PyQtサンプルプログラム")

        # ウィンドウのサイズを設定する
        self.setGeometry(0, 0, 640, 480)

        # ウィンドウを表示する
        self.show()

        # QTimerを初期化する
        timer = QTimer(self)

        # timeoutシグナルに「getDateTime()メソッド」をスロットとして指定する
        timer.timeout.connect(self.getDateTime) ##周期動作を設定

        # タイマーの動作を開始する
        timer.start()

    # getDateTime()メソッド｜ステータスバーに現在日時を表示する
    def getDateTime(self):
        # 現在の日時を取得する
        time = datetime.datetime.today()

        # 取得した日時をフォーマット済み文字列形式に変換する
        string = time.strftime(u"%Y年%m月%d日 %H時%M分%S秒")

        # ステータスバーに現在日時を表示する
        self.statusBar().showMessage(f"現在時刻：{string}")

if __name__ == "__main__":
    # PyQtを初期化する
    app = QApplication(sys.argv)

    # Programクラスのインスタンスを生成する
    tempChecker = TempChecker()
    tempChecker.show()
    # プログラムを終了する
    sys.exit(app.exec()) # pyqt5: sys.exit(app.exec_()) -> pyqt6 sys.exit(app.exec())
    """    
    qAp = QApplication(sys.argv)    
    tempChecker = TempChecker()
    tempChecker.show()
    qAp.exec()
   """ 
