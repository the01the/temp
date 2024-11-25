using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;


using Ghost.Utils;
using System.Collections.ObjectModel;
using Reactive.Bindings;        //コマンド作成
using System.Reactive.Subjects;
using System.Runtime.InteropServices.WindowsRuntime; //コマンド作成


using System.ComponentModel; //INotifyPropertyChangedで使用
using System.Runtime.CompilerServices; //CallerMemberNameで使用

using Ghost.Models;
using System.Collections;

namespace Ghost.ViewModels
{
    public class LotControlWindowViewModel : NotifyPropertyChanged
    {

        private Subject<bool> _SubjectCanExecuse;
        public ReactiveCommand BackButton_Pushed { get; set; }
        public ReactiveCommand SplitButton_Pushed { get; set; }
        public Action CloseWindowAction { get; set; }
        public string original_lot_num
        {
            get => GetPropertyValue("");
            set => RaisePropertyChangedIfSet(value);
        }
        public string original_serial_num
        {
            get => GetPropertyValue("");
            set => RaisePropertyChangedIfSet(value);
        }
        public string new_lot_num_first
        {
            get => GetPropertyValue("");
            set => RaisePropertyChangedIfSet(value);
        }
        public string new_serial_num_first
        {
            get => GetPropertyValue("");
            set => RaisePropertyChangedIfSet(value);
        }
        public string new_lot_num_second
        {
            get => GetPropertyValue("");
            set => RaisePropertyChangedIfSet(value);
        }
        public string new_serial_num_second
        {
            get => GetPropertyValue("");
            set => RaisePropertyChangedIfSet(value);
        }
        public string reason_of_split
        {
            get => GetPropertyValue("");
            set => RaisePropertyChangedIfSet(value);
        }
        public string comment_of_split
        {
            get => GetPropertyValue("");
            set => RaisePropertyChangedIfSet(value);
        }
        private ObservableCollection<string> _reasonList;
        public ObservableCollection<string> ReasonList
        {
            get { return _reasonList; }
            set
            {
                _reasonList = value;
                OnPropertyChanged(nameof(ReasonList));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public LotControlWindowViewModel() {
            //サブジェクトインスタンス作成
            _SubjectCanExecuse = new Subject<bool>();
            //各ツールのbind sourceのインスタンス作成
            // 作業指示表示欄

            BackButton_Pushed = _SubjectCanExecuse.ToReactiveCommand().WithSubscribe(async () => await FuncBackButton_Pushed());
            SplitButton_Pushed = _SubjectCanExecuse.ToReactiveCommand().WithSubscribe(async () => await FuncSplitButton_Pushed());

            //理由リスト作成
            ReasonList = new ObservableCollection<string>();
            ReasonList.Clear();
            ReasonList.Add("理由1");
            ReasonList.Add("理由2");
            ReasonList.Add("理由3");
            ReasonList.Add("その他");
        }

        private async Task FuncBackButton_Pushed()
        {
            var taskAsyncFunc = Task.Run(() =>
            {
                //処理の実行
                //viewのtextboxの文字列を読み込みアクティビティを実行
                //Model_ActivityToDownloadAllForCsv.FuncDownloadCsv();
            });
            await taskAsyncFunc;

            MessageBox.Show("ロット分割を中止します。");
        }
        private async Task FuncSplitButton_Pushed()
        {
            bool rlt = false;
            var taskAsyncFunc = Task.Run(() =>
            {
                //処理の実行
                rlt = lotControl();
            });
            await taskAsyncFunc;
            if (rlt != false)
            {
                CloseWindowAction();
            }
            else
            {
                //何もせず戻る
            }
        }
        private bool lotControl()
        {
            //入力内容の確認
            {
                if (original_lot_num == "")
                {
                    MessageBox.Show("分割元のロットNoが未入力です");
                    return false;
                }
                if (original_serial_num == "")
                {
                    MessageBox.Show("分割元のシリアルNoが未入力です");
                    return false;
                }
                if (new_lot_num_first == "")
                {
                    MessageBox.Show("分割先のロットNo①が未入力です");
                    return false;
                }
                if (new_serial_num_first == "")
                {
                    MessageBox.Show("分割先のシリアルNo①が未入力です");
                    return false;
                }
                if (new_lot_num_second == "")
                {
                    MessageBox.Show("分割先のロットNo②が未入力です");
                    return false;
                }
                if (new_serial_num_second == "")
                {
                    MessageBox.Show("分割先のシリアルNo②が未入力です");
                    return false;
                }
                if (reason_of_split == "")
                {
                    MessageBox.Show("分割元の分割理由が未選択です");
                    return false;
                }
                //備考欄は任意
            }
            
            //入力項目の検索
            {
                //DBで実績
                string query = "";
                query = $"" +
                                $"select Lot_Num from T_Achieve_Main " +
                                $"where " +
                                $"Basic_Code='{Ghost.ViewModels.MainWindowViewModel.TextForSearch_forAcheve}' " +
                                $"and Suffix='{Suffixitem_forAcheve}' " +
                                $"ans Lot_Num={original_lot_num} ;";
                AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                Collection<string> target = new Collection<string>();
                target.Add("Lot_Num");
                Collection<string> searchans = accessWorkLogDB.getReadStrings(query, target);
                if (searchans != null) {
                    MessageBox.Show("分割元のロットNoが見つかりません");
                    return false;
                }
            }
            //見つかった場合
            {

                Collection<int> lotspleted_main_ids = new Collection<int>();
                //過去の実績をロット番号、シリアル番号変えて登録しなおす
                {
                    //前のid実績IDを取得
                    Collection<string> id_list = new Collection<string>();

                    //分割①
                    {
                        Collection<int> insert_assy_id = new Collection<int>();
                        Collection<int> insert_lot_id = new Collection<int>();
                        Collection<int> insert_qhr_id = new Collection<int>();
                        {
                            //DBで実績
                            string query = "";
                            query = $"" +
                                            $"select Lot_Num from T_Achieve_Main " +
                                            $"where " +
                                            $"Basic_Code='{Ghost.ViewModels.MainWindowViewModel.TextForSearch_forAcheve}' " +
                                            $"and Suffix='{Suffixitem_forAcheve}' " +
                                            $"and Lot_Num={original_lot_num} ;";
                            AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                            Collection<string> target = new Collection<string>();
                            target.Add("Achieve_Lot_ID");
                            target.Add("Achieve_ASSY_ID");
                            target.Add("Achieve_QHR_ID");
                            id_list = accessWorkLogDB.getReadStrings(query, target);
                        }
                        //組み立て
                        {
                            /*
                             * https://qiita.com/amay077/items/ba27030e7009c16971ee
select * into #wktbl from tbl1
alter table #wktbl drop column id
insert into tbl2 select 1 as id ,* from #wktbl
                             */
                            AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                            string query = "";
                            query = $"select MAX(CONVERT(int , Achieve_ASSY_ID)) as 'max_num' from T_Achieve_ASSY; ";
                            Collection<string> target_col = new Collection<string>();
                            target_col.Add("max_num");
                            insert_assy_id = accessWorkLogDB.getReadNum(query, target_col);
                            //insert関数でクエリだけ実行
                            string updatequery = $"" +
                                $"select * into tmp_item  from T_Achieve_ASSY where Achieve_Lot_ID='{id_list[0].ToString()}'" +
                                $"alter table tmp_item drop column Achieve_ASSY_ID, " +
                                $"insert into T_Achieve_ASSY select {insert_assy_id[0] + 1} as Achieve_ASSY_ID ,* from tmp_item ";
                            accessWorkLogDB.insertData(query);
                        }
                        //品質
                        {
                            AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                            string query = "";
                            query = $"select MAX(CONVERT(int , Achieve_Lot_ID)) as 'max_num' from T_Achieve_Lot; ";
                            Collection<string> target_col = new Collection<string>();
                            target_col.Add("max_num");
                            insert_lot_id = accessWorkLogDB.getReadNum(query, target_col);//MPFに適用するECO
                            //insert関数でクエリだけ実行
                            string updatequery = $"" +
                                $"select * into tmp_item  from T_Achieve_Lot where Achieve_Lot_ID='{id_list[1].ToString()}'" +
                                $"alter table tmp_item drop column Achieve_Lot_ID, " +
                                $"insert into T_Achieve_Lot select {insert_lot_id[0] + 1} as Achieve_Lot_ID ,* from tmp_item ";
                            accessWorkLogDB.insertData(query);
                        }
                        {
                            AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                            string query = "";
                            query = $"select MAX(CONVERT(int , Achieve_QHR_ID)) as 'max_num' from T_Achieve_QHR; ";
                            Collection<string> target_col = new Collection<string>();
                            target_col.Add("max_num");
                            insert_qhr_id = accessWorkLogDB.getReadNum(query, target_col);//MPFに適用するECO
                            //insert関数でクエリだけ実行
                            string updatequery = $"" +
                                $"select * into tmp_item  from T_Achieve_QHR where Achieve_QHR_ID='{id_list[1].ToString()}'" +
                                $"alter table tmp_item drop column Achieve_QHR_ID, " +
                                $"insert into T_Achieve_QHR select {insert_qhr_id[0] + 1} as Achieve_QHR_ID ,* from tmp_item ";
                            accessWorkLogDB.insertData(query);
                        }
                        //qhr
                        {
                            AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                            string query = "";
                            query = $"select MAX(CONVERT(int , Achieve_QHR_ID)) as 'max_num' from T_Achieve_QHR; ";
                            Collection<string> target_col = new Collection<string>();
                            target_col.Add("max_num");
                            insert_qhr_id = accessWorkLogDB.getReadNum(query, target_col);//MPFに適用するECO
                            //insert関数でクエリだけ実行
                            string updatequery = $"" +
                                $"select * into tmp_item  from T_Achieve_QHR where Achieve_QHR_ID='{id_list[1].ToString()}'" +
                                $"alter table tmp_item drop column Achieve_QHR_ID, " +
                                $"insert into T_Achieve_QHR select {insert_qhr_id[0] + 1} as Achieve_QHR_ID ,* from tmp_item ";
                            accessWorkLogDB.insertData(query);
                        }

                        //実績メインテーブル登録
                        Collection<int> insert_mpfmain_id = new Collection<int>();
                        {
                            AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                            string query = "";
                            query = $"select MAX(CONVERT(int , Achieve_ID)) as 'max_num' from T_Achieve_Main; ";
                            Collection<string> target_col = new Collection<string>();
                            target_col.Add("max_num");
                            insert_mpfmain_id = accessWorkLogDB.getReadNum(query, target_col);//MPFに適用するECO
                            //insert関数でクエリだけ実行
                            string updatequery = $"" +
                                $"select * into tmp_item  from T_Achieve_Main" +
                                $" where " +
                                $"Basic_Code='{Ghost.ViewModels.MainWindowViewModel.TextForSearch_forAcheve}' " +
                                $"and Suffix='{Suffixitem_forAcheve}' " +
                                $"and Lot_Num={original_lot_num} " +
                                $"alter table tmp_item drop column Achieve_ID, Lot_Num ,Achieve_Lot_ID, Achieve_ASSY_ID, Achieve_QHR_ID, LotSplit_Memo" +
                                $"insert into T_Achieve_Main select {insert_mpfmain_id[0] + 1},{new_lot_num_first},{insert_assy_id[0] + 1},{insert_lot_id[0] + 1},{insert_qhr_id[0] + 1}, ''" +
                                $" as Achieve_ID, Lot_Num ,Achieve_Lot_ID, Achieve_ASSY_ID, Achieve_QHR_ID, LotSplit_Memo ,* from tmp_item ";
                            accessWorkLogDB.insertData(query);

                            lotspleted_main_ids.Add(insert_mpfmain_id[0] + 1);
                            //☆シリアルが無い？
                            //☆メインテーブルのinsert_mpfmain_idで+1していない？
                        }
                    }

                    //分割②
                    {
                        Collection<int> insert_assy_id = new Collection<int>();
                        Collection<int> insert_lot_id = new Collection<int>();
                        Collection<int> insert_qhr_id = new Collection<int>();
                        {
                            //DBで実績
                            string query = "";
                            query = $"" +
                                            $"select Lot_Num from T_Achieve_Main " +
                                            $"where " +
                                            $"Basic_Code='{Ghost.ViewModels.MainWindowViewModel.TextForSearch_forAcheve}' " +
                                            $"and Suffix='{Suffixitem_forAcheve}' " +
                                            $"and Lot_Num={original_lot_num} ;";
                            AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                            Collection<string> target = new Collection<string>();
                            target.Add("Achieve_Lot_ID");
                            target.Add("Achieve_ASSY_ID");
                            target.Add("Achieve_QHR_ID");
                            id_list = accessWorkLogDB.getReadStrings(query, target);
                        }
                        //組み立て
                        {
                            /*
                             * https://qiita.com/amay077/items/ba27030e7009c16971ee
select * into #wktbl from tbl1
alter table #wktbl drop column id
insert into tbl2 select 1 as id ,* from #wktbl
                             */
                            AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                            string query = "";
                            query = $"select MAX(CONVERT(int , Achieve_ASSY_ID)) as 'max_num' from T_Achieve_ASSY; ";
                            Collection<string> target_col = new Collection<string>();
                            target_col.Add("max_num");
                            insert_assy_id = accessWorkLogDB.getReadNum(query, target_col);
                            //insert関数でクエリだけ実行
                            string updatequery = $"" +
                                $"select * into tmp_item  from T_Achieve_ASSY where Achieve_Lot_ID='{id_list[0].ToString()}'" +
                                $"alter table tmp_item drop column Achieve_ASSY_ID, " +
                                $"insert into T_Achieve_ASSY select {insert_assy_id[0] + 1} as Achieve_ASSY_ID ,* from tmp_item ";
                            accessWorkLogDB.insertData(query);
                        }
                        //品質
                        {
                            AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                            string query = "";
                            query = $"select MAX(CONVERT(int , Achieve_Lot_ID)) as 'max_num' from T_Achieve_Lot; ";
                            Collection<string> target_col = new Collection<string>();
                            target_col.Add("max_num");
                            insert_lot_id = accessWorkLogDB.getReadNum(query, target_col);//MPFに適用するECO
                            //insert関数でクエリだけ実行
                            string updatequery = $"" +
                                $"select * into tmp_item  from T_Achieve_Lot where Achieve_Lot_ID='{id_list[1].ToString()}'" +
                                $"alter table tmp_item drop column Achieve_Lot_ID, " +
                                $"insert into T_Achieve_Lot select {insert_lot_id[0] + 1} as Achieve_Lot_ID ,* from tmp_item ";
                            accessWorkLogDB.insertData(query);
                        }
                        {
                            AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                            string query = "";
                            query = $"select MAX(CONVERT(int , Achieve_QHR_ID)) as 'max_num' from T_Achieve_QHR; ";
                            Collection<string> target_col = new Collection<string>();
                            target_col.Add("max_num");
                            insert_qhr_id = accessWorkLogDB.getReadNum(query, target_col);//MPFに適用するECO
                            //insert関数でクエリだけ実行
                            string updatequery = $"" +
                                $"select * into tmp_item  from T_Achieve_QHR where Achieve_QHR_ID='{id_list[1].ToString()}'" +
                                $"alter table tmp_item drop column Achieve_QHR_ID, " +
                                $"insert into T_Achieve_QHR select {insert_qhr_id[0] + 1} as Achieve_QHR_ID ,* from tmp_item ";
                            accessWorkLogDB.insertData(query);
                        }
                        //qhr
                        {
                            AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                            string query = "";
                            query = $"select MAX(CONVERT(int , Achieve_QHR_ID)) as 'max_num' from T_Achieve_QHR; ";
                            Collection<string> target_col = new Collection<string>();
                            target_col.Add("max_num");
                            insert_qhr_id = accessWorkLogDB.getReadNum(query, target_col);//MPFに適用するECO
                            //insert関数でクエリだけ実行
                            string updatequery = $"" +
                                $"select * into tmp_item  from T_Achieve_QHR where Achieve_QHR_ID='{id_list[1].ToString()}'" +
                                $"alter table tmp_item drop column Achieve_QHR_ID, " +
                                $"insert into T_Achieve_QHR select {insert_qhr_id[0] + 1} as Achieve_QHR_ID ,* from tmp_item ";
                            accessWorkLogDB.insertData(query);
                        }

                        //実績メインテーブル登録
                        Collection<int> insert_mpfmain_id = new Collection<int>();
                        {
                            AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                            string query = "";
                            query = $"select MAX(CONVERT(int , Achieve_ID)) as 'max_num' from T_Achieve_Main; ";
                            Collection<string> target_col = new Collection<string>();
                            target_col.Add("max_num");
                            insert_mpfmain_id = accessWorkLogDB.getReadNum(query, target_col);//MPFに適用するECO
                            //insert関数でクエリだけ実行
                            string updatequery = $"" +
                                $"select * into tmp_item  from T_Achieve_Main" +
                                $" where " +
                                $"Basic_Code='{Ghost.ViewModels.MainWindowViewModel.TextForSearch_forAcheve}' " +
                                $"and Suffix='{Suffixitem_forAcheve}' " +
                                $"and Lot_Num={original_lot_num} " +
                                $"alter table tmp_item drop column Achieve_ID, Lot_Num ,Achieve_Lot_ID, Achieve_ASSY_ID, Achieve_QHR_ID, LotSplit_Memo" +
                                $"insert into T_Achieve_Main select {insert_mpfmain_id[0] + 1},{new_lot_num_second},{insert_assy_id[0] + 1},{insert_lot_id[0] + 1},{insert_qhr_id[0] + 1}, ''" +
                                $" as Achieve_ID, Lot_Num ,Achieve_Lot_ID, Achieve_ASSY_ID, Achieve_QHR_ID, LotSplit_Memo ,* from tmp_item ";
                            accessWorkLogDB.insertData(query);
                            lotspleted_main_ids.Add(insert_mpfmain_id[0] + 1);
                            //☆シリアルが無い？
                            //☆メインテーブルのinsert_mpfmain_idで+1していない？
                        }
                    }
                }
                //成績書出力でのデータ収集でnextがあればそっちにするためのフラグを立てる
                {
                    AccessWorkLogDB accessWorkLogDB = new AccessWorkLogDB();
                    string query = "";
                    query = $"update T_Achieve_Main " +
                            $"set LotSplit_Memo='{lotspleted_main_ids[0]}, {lotspleted_main_ids[1]}'" + //分割先の番号
                            $" where " +
                            $"Basic_Code='{Ghost.ViewModels.MainWindowViewModel.TextForSearch_forAcheve}' " +
                            $"and Suffix='{Suffixitem_forAcheve}' " +
                            $"and Lot_Num={original_lot_num} " +
                            $";";
                    accessWorkLogDB.insertData(query);
                }
            }
            return true;
        }
    }
}
