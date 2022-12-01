List<Invoice> invoices = new List<Invoice>() { 
    new Invoice()
    {
        INVOICE_NO = "INV221116B",
        ORDER_NO = "PR221100101",
        MDL_CAT = "ZZZ",
        PART_NO = "KRP4A98",
        QTY = 5,
    },
    new Invoice()
    {
        INVOICE_NO = "INV221116B",
        ORDER_NO = "PR221100201",
        MDL_CAT = "ZZZ",
        PART_NO = "KRP4A98",
        QTY = 10
    },
    new Invoice()
    {
        INVOICE_NO = "INV221116A",
        ORDER_NO = "PR221100101",
        MDL_CAT = "ZZZ",
        PART_NO = "KCD-250D1",
        QTY = 10
    }
};

List<Container> containers = new List<Container>() {
    new Container()
    {
        INVOICE_NO = "INV221116B",
        ORDER_NO = "PR221100201,PR221100101",
        MDL_CAT = "ZZZ",
        PART_NO = "KRP4A98",
        CONT_QTY = 3,
        CONT_NO = "KGBU7282663",
        IS_DONE = false
    },
    new Container()
    {
        INVOICE_NO = "INV221116B",
        ORDER_NO = "PR221100201,PR221100101",
        MDL_CAT = "ZZZ",
        PART_NO = "KRP4A98",
        CONT_QTY = 12,
        CONT_NO = "TGBU7282663",
        IS_DONE = false
    },
    new Container()
    {
        INVOICE_NO = "INV221116A",
        ORDER_NO = "PR221100101",
        MDL_CAT = "ZZZ",
        PART_NO = "KCD-250D1",
        CONT_QTY = 11,
        CONT_NO = "EGHU9513033",
        IS_DONE = false
    }
};

containers = containers.OrderByDescending(p => p.CONT_QTY).ToList();

var data = GetContainer(containers, invoices);

Console.WriteLine(123);

static List<Container> GetContainer(List<Container> oriContainers, List<Invoice> invoices)
{
    List<Container> output = new List<Container>();

    foreach (var item in oriContainers)
    {
        var orderList = item.ORDER_NO.Split(',').Select(p => p.Trim()).ToList();

        var invoice = invoices
            .Where(p => 
                p.INVOICE_NO == item.INVOICE_NO && 
                p.MDL_CAT == item.MDL_CAT &&
                p.PART_NO == item.PART_NO &&
                orderList.Contains(p.ORDER_NO) &&
                p.QTY > 0)
            .OrderByDescending(p => p.QTY)
            .FirstOrDefault();

        if (invoice != null && item.IS_DONE == false)
        {
            if (item.CONT_QTY > invoice.QTY)
            {
                output.Add(new Container
                {
                    INVOICE_NO = item.INVOICE_NO,
                    MDL_CAT = item.MDL_CAT,
                    PART_NO = item.PART_NO,
                    CONT_NO = item.CONT_NO,
                    ORDER_NO = invoice.ORDER_NO,
                    CONT_QTY = invoice.QTY
                });

                item.CONT_QTY = item.CONT_QTY - invoice.QTY;

                invoice.QTY = 0;

                var rtnOutput = GetContainer(oriContainers, invoices);

                output = output.Union(rtnOutput).ToList();
            }
            else
            {
                output.Add(new Container
                {
                    INVOICE_NO = item.INVOICE_NO,
                    MDL_CAT = item.MDL_CAT,
                    PART_NO = item.PART_NO,
                    CONT_NO = item.CONT_NO,
                    ORDER_NO = invoice.ORDER_NO,
                    CONT_QTY = item.CONT_QTY
                });

                invoice.QTY = invoice.QTY - item.CONT_QTY;

                item.CONT_QTY = 0;

                item.IS_DONE = true;
            }
        }
    }

    //  數量需要相等，若取完數量後有任一(Invoice, 裝櫃)的數量不為0，就要提示
    if (oriContainers.Any(p => p.CONT_QTY != 0) || invoices.Any(p => p.QTY != 0))
    {
        var invalidOriContainers = oriContainers.Where(p => p.CONT_QTY != 0).SelectMany(
            items => items.ORDER_NO.Split(',').Select(s => s.Trim()),
            (item, order) => new
            {
                ORDER_NO = order,
                INVOICE_NO = item.INVOICE_NO,
                PART_NO = item.PART_NO,
                MDL_CAT = item.MDL_CAT ?? "ZZZ"
            })
        .GroupBy(a => new { a.ORDER_NO, a.INVOICE_NO, a.PART_NO, a.MDL_CAT })
        .Select(p => new
        {
            ORDER_NO = p.Key.ORDER_NO,
            INVOICE_NO = p.Key.INVOICE_NO,
            PART_NO = p.Key.PART_NO,
            MDL_CAT = p.Key.MDL_CAT
        })
        .ToList();

        var invalidInvioces = invoices.Where(p => p.QTY != 0).Select(p => new {
            ORDER_NO = p.ORDER_NO,
            INVOICE_NO = p.INVOICE_NO,
            PART_NO = p.PART_NO,
            MDL_CAT = p.MDL_CAT ?? "ZZZ"
        });

        var invalidData = invalidInvioces.Concat(invalidOriContainers);
    }

    return output;
}





public class Invoice
{
    public string INVOICE_NO { get; set; }
    public string ORDER_NO { get; set; }
    public string MDL_CAT { get; set; }
    public string PART_NO { get; set; }
    public int QTY { get; set; }
}

public class Container
{
    public string INVOICE_NO { get; set; }
    public string ORDER_NO { get; set; }
    public string MDL_CAT { get; set; }
    public string PART_NO { get; set; }
    public string CONT_NO { get; set; }
    public int CONT_QTY { get; set; }
    public bool IS_DONE { get; set; } = false;
}