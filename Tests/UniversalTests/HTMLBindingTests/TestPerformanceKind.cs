namespace Tests.Universal.HTMLBindingTests
{
    public enum TestPerformanceKind 
    {
        //Ko-Cef context: 1500
        OneTime_Collection_CreateBinding,
        //Ko-Cef context: 1500
        TwoWay_Collection_CreateBinding,
        //Ko-Cef context: 1500
        OneWay_Collection_CreateBinding,
        //Ko-Cef context: 3100
        TwoWay_Int,
        //Ko-Cef context: 4700
        TwoWay_Collection,

        TwoWay_Collection_Update_From_Javascript
    }
}
