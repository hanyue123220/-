using System.Collections.Generic;
using System.Xml.Serialization;
using 视觉检测系统.业务流程.方案.保存方案;

[XmlRoot("PlanList")]
public class PlanList
{
    [XmlArray("Plans"), XmlArrayItem("Plan")]
    public List<PlanItem> Items { get; set; } = new List<PlanItem>();
}