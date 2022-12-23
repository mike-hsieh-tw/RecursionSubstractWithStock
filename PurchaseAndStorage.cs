using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    /// <summary>
    /// 邏輯說明: 
    ///     1.  PurchaseModel 為要分配到各儲位的資料，其中的QTY為總數量。
    ///     2.  StorageRuleModel 為儲位的設定數量和百分比。
    ///     3.  對 PurchaseModel 遍瀝，並且當下每筆資料去比對 儲位設定資料，有資料就進行分配，直到儲位設定的數量被取完。
    /// </summary>
    /// <param name="storageRuleModels"></param>
    /// <param name="purchaseModels"></param>
    /// <returns></returns>
    internal class PurchaseAndStorage
    {
        List<StorageRuleModel> storageRuleModels = new List<StorageRuleModel>()
        {
            new StorageRuleModel { CONT_NO ="EGHU9513033", IN_NO ="IN221116B", IN_NO1 ="02", INVOICE_NO ="INV221116B", ORDER_NO ="PR221100301", MDL_CAT ="ZZZ", PART_NO ="KCD-250D1", LCT_NO ="21", ALLOT_QTY = 1, ALLOT_RATE = 0.330000 },
            new StorageRuleModel { CONT_NO ="EGHU9513033", IN_NO ="IN221116A", IN_NO1 ="01", INVOICE_NO ="INV221116A", ORDER_NO ="PR221100101", MDL_CAT ="ZZZ", PART_NO ="KCD-250D1", LCT_NO ="ZZ", ALLOT_QTY = 2, ALLOT_RATE = 0.200000 },
            new StorageRuleModel { CONT_NO ="EGHU9513033", IN_NO ="IN221116B", IN_NO1 ="02", INVOICE_NO ="INV221116B", ORDER_NO ="PR221100301", MDL_CAT ="ZZZ", PART_NO ="KCD-250D1", LCT_NO ="ZZ", ALLOT_QTY = 2, ALLOT_RATE = 0.670000 },
            new StorageRuleModel { CONT_NO ="EGHU9513033", IN_NO ="IN221116A", IN_NO1 ="01", INVOICE_NO ="INV221116A", ORDER_NO ="PR221100101", MDL_CAT ="ZZZ", PART_NO ="KCD-250D1", LCT_NO ="21", ALLOT_QTY = 3, ALLOT_RATE = 0.300000 },
            new StorageRuleModel { CONT_NO ="EGHU9513033", IN_NO ="IN221116A", IN_NO1 ="01", INVOICE_NO ="INV221116A", ORDER_NO ="PR221100101", MDL_CAT ="ZZZ", PART_NO ="KCD-250D1", LCT_NO ="ZA", ALLOT_QTY = 5, ALLOT_RATE = 0.500000 },
            new StorageRuleModel { CONT_NO ="EGHU9513033", IN_NO ="IN221116A", IN_NO1 ="01", INVOICE_NO ="INV221116A", ORDER_NO ="PR221100201", MDL_CAT ="ZZZ", PART_NO ="KDBD63A160", LCT_NO ="ZZ", ALLOT_QTY = 1, ALLOT_RATE = 0.100000 },
            new StorageRuleModel { CONT_NO ="EGHU9513033", IN_NO ="IN221116A", IN_NO1 ="01", INVOICE_NO ="INV221116A", ORDER_NO ="PR221100201", MDL_CAT ="ZZZ", PART_NO ="KDBD63A160", LCT_NO ="21", ALLOT_QTY = 2, ALLOT_RATE = 0.200000 },
            new StorageRuleModel { CONT_NO ="EGHU9513033", IN_NO ="IN221116A", IN_NO1 ="01", INVOICE_NO ="INV221116A", ORDER_NO ="PR221100201", MDL_CAT ="ZZZ", PART_NO ="KDBD63A160", LCT_NO ="ZA", ALLOT_QTY = 7, ALLOT_RATE = 0.700000 },
            new StorageRuleModel { CONT_NO ="EGHU9513033", IN_NO ="IN221116A", IN_NO1 ="01", INVOICE_NO ="INV221116A", ORDER_NO ="PR221100101", MDL_CAT ="ZZZ", PART_NO ="KDDF37AA80", LCT_NO ="21", ALLOT_QTY = 2, ALLOT_RATE = 0.200000 },
            new StorageRuleModel { CONT_NO ="EGHU9513033", IN_NO ="IN221116A", IN_NO1 ="01", INVOICE_NO ="INV221116A", ORDER_NO ="PR221100101", MDL_CAT ="ZZZ", PART_NO ="KDDF37AA80", LCT_NO ="ZZ", ALLOT_QTY = 2, ALLOT_RATE = 0.200000 },
            new StorageRuleModel { CONT_NO ="EGHU9513033", IN_NO ="IN221116A", IN_NO1 ="01", INVOICE_NO ="INV221116A", ORDER_NO ="PR221100101", MDL_CAT ="ZZZ", PART_NO ="KDDF37AA80", LCT_NO ="ZA", ALLOT_QTY = 6, ALLOT_RATE = 0.600000 }
        };

        List<PurchaseModel> purchaseModels = new List<PurchaseModel>()
        {
            new PurchaseModel { CONT_NO ="EGHU9513033", MDL_CAT ="ZZZ", PART_NO ="KCD-250D1", QTY = 13 },
            new PurchaseModel { CONT_NO ="EGHU9513033", MDL_CAT ="ZZZ", PART_NO ="KDBD63A160", QTY = 10 },
            new PurchaseModel { CONT_NO ="EGHU9513033", MDL_CAT ="ZZZ", PART_NO ="KDDF37AA80", QTY = 10 },
        };

        public void GoGetStorage()
        {
            var result = GetStorage(storageRuleModels, ref purchaseModels);
            var rtnPurchaseModels = purchaseModels;
        }

        /// <summary>
        /// 邏輯說明: 
        ///     1.  PurchaseModel 為要分配到各儲位的資料，其中的QTY為總數量。
        ///     2.  StorageRuleModel 為儲位的設定數量和百分比。
        ///     3.  對 PurchaseModel 遍瀝，並且當下每筆資料去比對 儲位設定資料，有資料就進行分配，直到儲位設定的數量被取完。
        /// </summary>
        /// <param name="storageRuleModels"></param>
        /// <param name="purchaseModels"></param>
        /// <returns></returns>
        static List<StorageRuleModel> GetStorage(List<StorageRuleModel> storageRuleModels, ref List<PurchaseModel> purchaseModels)
        {
            //  回傳集合
            List<StorageRuleModel> output = new List<StorageRuleModel>();

            foreach (var item in purchaseModels)
            {
                //  取得儲位設定的資料
                var storageRuleModel = storageRuleModels
                    .Where(
                        p =>
                        p.CONT_NO == item.CONT_NO &&
                        p.MDL_CAT == item.MDL_CAT &&
                        p.PART_NO == item.PART_NO &&
                        p.ALLOT_QTY > 0)
                    .FirstOrDefault();

                //  若有無儲位設定的資料 且 原分配資料尚未結束
                if (storageRuleModel != null && item.IS_DONE == false)
                {
                    //  未分儲數量 > 分儲設定的數量。表示該儲位已經分配完
                    if (item.QTY > storageRuleModel.ALLOT_QTY)
                    {
                        //   表示該儲位已經分配完，直接加入到回傳集合中
                        output.Add(new StorageRuleModel {
                            CONT_NO = item.CONT_NO,
                            IN_NO = storageRuleModel.IN_NO,
                            IN_NO1 = storageRuleModel.IN_NO1,
                            INVOICE_NO = storageRuleModel.INVOICE_NO,
                            ORDER_NO = storageRuleModel.ORDER_NO,
                            MDL_CAT = item.MDL_CAT,
                            PART_NO = item.PART_NO,
                            LCT_NO = storageRuleModel.LCT_NO,
                            ALLOT_QTY = storageRuleModel.ALLOT_QTY, 
                            ALLOT_RATE = storageRuleModel.ALLOT_RATE,
                            ACT_PURCHASE_QTY = storageRuleModel.ALLOT_QTY   //  實際已分配，改為分配數量
                        });

                        item.QTY = item.QTY - storageRuleModel.ALLOT_QTY;   //  已分配取完了，剩餘未分配改為差值

                        storageRuleModel.ALLOT_QTY = 0;     //  已分配取完了，剩餘未分配改為 0

                        //  尚未非配到儲位的資料，繼續進行分配
                        var rtnOutput = GetStorage(storageRuleModels, ref purchaseModels);

                        //  合併資料
                        output = output.Union(rtnOutput).ToList();
                    }
                    else
                    {
                        //  表示該儲位都已經分配完，直接加入到回傳集合中
                        output.Add(new StorageRuleModel
                        {
                            CONT_NO = item.CONT_NO,
                            IN_NO = storageRuleModel.IN_NO,
                            IN_NO1 = storageRuleModel.IN_NO1,
                            INVOICE_NO = storageRuleModel.INVOICE_NO,
                            ORDER_NO = storageRuleModel.ORDER_NO,
                            MDL_CAT = item.MDL_CAT,
                            PART_NO = item.PART_NO,
                            LCT_NO = storageRuleModel.LCT_NO,
                            ALLOT_QTY = storageRuleModel.ALLOT_QTY, //  設定檔分配數量
                            ALLOT_RATE = storageRuleModel.ALLOT_RATE,
                            ACT_PURCHASE_QTY = item.QTY,   //  實際已分配，改為分配數量
                        });

                        storageRuleModel.ALLOT_QTY = storageRuleModel.ALLOT_QTY - item.QTY;     //  已分配取完了，剩餘未分配改為 差值

                        item.QTY = 0;   //  已分配取完了，改為0

                        item.IS_DONE = true;    //  分配結束
                    }
                }
            }

            //  還有沒分配完的數量，表示超過分儲定義的分配量
            if (purchaseModels.Any(p => p.QTY > 0))
            {
                purchaseModels = purchaseModels.Select(p => {
                    p.ErrorMsg = p.QTY > 0 ? $"超過分儲總數量{p.QTY}個" : null;
                    return p;
                }).ToList();
            }

            return output;
        }

    }

    public class StorageRuleModel
    {
        public string CONT_NO       { get; set; }
        public string IN_NO         { get; set; }
        public string IN_NO1        { get; set; }
        public string INVOICE_NO    { get; set; }
        public string ORDER_NO      { get; set; }
        public string MDL_CAT       { get; set; }
        public string PART_NO       { get; set; }
        public string LCT_NO        { get; set; }

        //  原始定義要分配的數量
        public decimal ALLOT_QTY     { get; set; }
        public double ALLOT_RATE { get; set; }

        //  實際被分配到的數量
        public decimal ACT_PURCHASE_QTY { get; set; } = 0;
    }

    public class PurchaseModel
    {
        public string CONT_NO {get; set; }
        public string MDL_CAT {get; set; }
        public string PART_NO {get; set; }

        //  總共要分配的數量
        public decimal QTY { get; set; }
        public bool IS_DONE { get; set; } = false;

        public string ErrorMsg { get; set; }
    }
}
