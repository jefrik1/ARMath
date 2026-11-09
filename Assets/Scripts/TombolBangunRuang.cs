using UnityEngine;

public class TombolBangunRuang : MonoBehaviour
{
    public void KlikInfo()
    {
        if (TargetBangunRuang.Aktif != null && UIBangunRuang.Instance != null)
        {
            UIBangunRuang.Instance.TampilInfo(TargetBangunRuang.Aktif.jenisBangun);
            TargetBangunRuang.Aktif.TampilModelUtuh(); // Info sambil tampil model utuh
        }
    }

    public void KlikRumus()
    {
        if (TargetBangunRuang.Aktif != null && UIBangunRuang.Instance != null)
        {
            TargetBangunRuang.Aktif.TampilRumus();
            UIBangunRuang.Instance.TampilRumus(TargetBangunRuang.Aktif.jenisBangun);        }
    }

    public void KlikRusuk()
    {
        if (TargetBangunRuang.Aktif != null && UIBangunRuang.Instance != null)
        {
            TargetBangunRuang.Aktif.TampilRusuk();
            UIBangunRuang.Instance.TampilRusuk(TargetBangunRuang.Aktif.jenisBangun);
        }
    }

    public void KlikJaring()
    {
        if (TargetBangunRuang.Aktif != null && UIBangunRuang.Instance != null)
        {
            TargetBangunRuang.Aktif.TampilJaring();
            UIBangunRuang.Instance.TampilJaring(TargetBangunRuang.Aktif.jenisBangun);
        }
    }
}