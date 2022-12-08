using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class InvoiceAndContainer
    {
        //  商品的發票資料
        List<Invoice> invoices = new List<Invoice>() {
            new Invoice()
            {
                INVOICE_NO = "INV221116B",
                ORDER_NO = "PR221100101",
                TYPE = "ZZZ",
                PROD_NO = "Orange",
                QTY = 5,
            },
            new Invoice()
            {
                INVOICE_NO = "INV221116B",
                ORDER_NO = "PR221100201",
                TYPE = "ZZZ",
                PROD_NO = "Orange",
                QTY = 10
            },
            new Invoice()
            {
                INVOICE_NO = "INV221116A",
                ORDER_NO = "PR221100101",
                TYPE = "ZZZ",
                PROD_NO = "Apple",
                QTY = 10
            }
        };

        //  商品的裝櫃資料
        List<Container> containers = new List<Container>() {
            new Container()
            {
                INVOICE_NO = "INV221116B",
                ORDER_NO = "PR221100201,PR221100101",               //  由於訂單編號是由逗號相加，所以不會知道要分的數量有多少，須走遞迴
                TYPE = "ZZZ",
                PROD_NO = "Orange",
                CONT_QTY = 3,
                CONT_NO = "CONT_NO_001",
                IS_DONE = false
            },
            new Container()
            {
                INVOICE_NO = "INV221116B",
                ORDER_NO = "PR221100201,PR221100101",               //  由於訂單編號是由逗號相加，所以不會知道要分的數量有多少，須走遞迴
                TYPE = "ZZZ",
                PROD_NO = "Orange",
                CONT_QTY = 12,
                CONT_NO = "CONT_NO_002",
                IS_DONE = false
            },
            new Container()
            {
                INVOICE_NO = "INV221116A",
                ORDER_NO = "PR221100101",
                TYPE = "ZZZ",
                PROD_NO = "Apple",
                CONT_QTY = 10,
                CONT_NO = "CONT_NO_003",
                IS_DONE = false
            }
        };

        public void GoGetContainer()
        {
            containers = containers.OrderByDescending(p => p.CONT_QTY).ToList();

            var data = GetContainer(containers, invoices);

            Console.WriteLine("Done");
        }

        /// <summary>
        /// 邏輯說明: 
        ///     0.  由於該邏輯是用戶需要使用 Excel 匯入，其中的 ORDER_NO 訂單編號是由逗號相加，所以不會知道要拆分的數量有多少，須走遞迴取得。
        ///     1.  Orange 在發票檔中共15個(10 + 5)，但是裝櫃時要拆成3個於CONT_NO_001櫃，另外12個於CONT_NO_002，此時會面臨到底前3個要取哪個發票資料，後面12個還要拆成2個和10個取發票的資料，此時就需要走遞迴去用扣的方式取得。
        ///     2.  Apple 在裝櫃檔中有 11 個，但是發票中只有 10 個，所以數量不吻合，會提示錯誤。若改為10個就可以順利分配。
        ///     
        ///     重點: 該拆分需要從大的先拆起，否則有可能小的先拆造成過度分散的結果
        /// </summary>
        static List<Container> GetContainer(List<Container> oriContainers, List<Invoice> invoices)
        {
            //  回傳集合
            List<Container> output = new List<Container>();

            foreach (var item in oriContainers)
            {
                //  將訂單編號依照逗號拆分
                var orderList = item.ORDER_NO.Split(',').Select(p => p.Trim()).ToList();

                //  取得有對應到PKEY且有數量的發票資料，數量大到小排序
                var invoice = invoices
                    .Where(p =>
                        //  依照複合組件取得對應的發票資料
                        p.INVOICE_NO == item.INVOICE_NO && p.TYPE == item.TYPE && p.PROD_NO == item.PROD_NO && orderList.Contains(p.ORDER_NO) &&
                        p.QTY > 0)  //  取得還有剩餘可以分配的數量
                    .OrderByDescending(p => p.QTY)  //  數量大到小 (該拆分需要從大的先拆起，否則有可能小的先拆造成過度分散的結果)
                    .FirstOrDefault();

                /// 若是有取得資料，表示:
                ///     1.  有對應的 Key 值
                ///     2.  還有剩餘數量
                ///     3.  當前的裝櫃還沒有分完 (item.IS_DONE == false)
                if (invoice != null && item.IS_DONE == false)
                {
                    ///  如果 裝櫃商品數量 > 發票商品數量，表示:
                    ///     1.  當前的發票數量會被分完，剩餘0
                    ///     2.  還沒分完就會遞迴繼續分，會找下一個有對應到PKEY且有數量的發票資料
                    if (item.CONT_QTY > invoice.QTY)
                    {
                        //  當前的回傳集合 合併 已被分配完的資料
                        output.Add(new Container
                        {
                            INVOICE_NO = item.INVOICE_NO,
                            TYPE = item.TYPE,
                            PROD_NO = item.PROD_NO,
                            CONT_NO = item.CONT_NO,
                            ORDER_NO = invoice.ORDER_NO,
                            CONT_QTY = invoice.QTY
                        });

                        //  當前的裝櫃數量 為 扣完 [發票數量] 後的 未分配[裝櫃數量]，之後遞迴就會使用該未分配的 [裝櫃數量] 分配
                        item.CONT_QTY = item.CONT_QTY - invoice.QTY;

                        //  發票數量已被取完，為0
                        invoice.QTY = 0;

                        //  遞迴把沒有分完的數量分完
                        var rtnOutput = GetContainer(oriContainers, invoices);

                        //  當前的回傳集合 合併 還沒分完的回傳集合。
                        output = output.Union(rtnOutput).ToList();
                    }
                    ///  如果 裝櫃商品數量 < 發票商品數量，表示:
                    ///     1.  當前的裝櫃數量可以被取完，裝櫃數量為 0 (item.CONT_QTY = 0)
                    ///     2.  此時發票數量扣完後有可能有剩餘數量，但是裝櫃已被分配完，該筆資料就已經處理完(item.IS_DONE = true)
                    else
                    {
                        //  當前的回傳集合 合併 
                        output.Add(new Container
                        {
                            INVOICE_NO = item.INVOICE_NO,
                            TYPE = item.TYPE,
                            PROD_NO = item.PROD_NO,
                            CONT_NO = item.CONT_NO,
                            ORDER_NO = invoice.ORDER_NO,
                            CONT_QTY = item.CONT_QTY
                        });

                        //  此時發票數量扣完後理應為0，但有可能有剩餘數量。若有剩餘數量於最後會篩選出來並報錯
                        invoice.QTY = invoice.QTY - item.CONT_QTY;

                        //  當前的裝櫃數量可以被取完，裝櫃數量為 0
                        item.CONT_QTY = 0;

                        //  裝櫃已被分配完
                        item.IS_DONE = true;
                    }
                }
            }

            //  裝櫃和發票的數量需要相等，若取完數量後有任一(Invoice, 裝櫃)的數量不為0，就表示裝櫃有多，或是發票有少，就要提示
            if (oriContainers.Any(p => p.CONT_QTY != 0) || invoices.Any(p => p.QTY != 0))
            {
                //  取得沒有被分配完的裝櫃數量，GroupBy Pkey
                var invalidOriContainers = oriContainers.Where(p => p.CONT_QTY != 0).SelectMany(
                    items => items.ORDER_NO.Split(',').Select(s => s.Trim()),
                    (item, order) => new
                    {
                        ORDER_NO = order,
                        INVOICE_NO = item.INVOICE_NO,
                        PROD_NO = item.PROD_NO,
                        TYPE = item.TYPE ?? "ZZZ"
                    })
                .GroupBy(a => new { a.ORDER_NO, a.INVOICE_NO, a.PROD_NO, a.TYPE })
                .Select(p => new
                {
                    ORDER_NO = p.Key.ORDER_NO,
                    INVOICE_NO = p.Key.INVOICE_NO,
                    PROD_NO = p.Key.PROD_NO,
                    TYPE = p.Key.TYPE
                })
                .ToList();

                //  取得沒有被分配完的發票數量，GroupBy Pkey
                var invalidInvioces = invoices.Where(p => p.QTY != 0).Select(p => new {
                    ORDER_NO = p.ORDER_NO,
                    INVOICE_NO = p.INVOICE_NO,
                    PROD_NO = p.PROD_NO,
                    TYPE = p.TYPE ?? "ZZZ"
                });

                //  沒有被分配完的發票數量 組合 裝櫃數量
                var invalidData = invalidInvioces.Concat(invalidOriContainers);

                //  顯示錯誤
                throw new Exception($"匯入失敗，請檢查數量正確後再匯入。商品代號: {string.Join(',', invalidData.Select(p => p.PROD_NO)) }");
            }

            return output;
        }
    }

    public class Invoice
    {
        //  發票號碼
        public string INVOICE_NO { get; set; }

        //  訂單編號
        public string ORDER_NO { get; set; }

        //  種類
        public string TYPE { get; set; }

        //  商品代號
        public string PROD_NO { get; set; }

        //  數量
        public int QTY { get; set; }
    }

    public class Container
    {
        //  發票號碼
        public string INVOICE_NO { get; set; }

        //  訂單編號
        public string ORDER_NO { get; set; }

        //  種類
        public string TYPE { get; set; }

        //  商品代號
        public string PROD_NO { get; set; }

        //  裝櫃編號
        public string CONT_NO { get; set; }

        //  裝櫃編號
        public int CONT_QTY { get; set; }

        //  是否完成
        public bool IS_DONE { get; set; } = false;
    }
}
