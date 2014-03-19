using System.ComponentModel;

namespace OmssaLib
{
    public enum InputFileType
    {
        [Description("-f")]
        SingleDta,
        [Description("-fx")]
        MultipleDtaXml,
        [Description("-fb")]
        MultipleDta,
        [Description("-fp")]
        Pkl,
        [Description("-fm")]
        Mgf,
        [Description("-foms")]
        Oms,
        [Description("-fomx")]
        Omx,
        [Description("-fbz2")]
        OmxBzip2,
        [Description("-fxml")]
        OmssaXml,
    }
}
