namespace Bingora.Domain.ValueObjects;

public class ColumnRange
{
    public string ColumnName { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    
    public ColumnRange(){}

    public ColumnRange(string columnName, int minValue, int maxValue)
    {
        ColumnName =  columnName;
        MinValue = minValue;
        MaxValue = maxValue;
    }
}