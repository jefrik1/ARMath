using UnityEngine;
using TMPro;

public class UIBangunRuang : MonoBehaviour
{
    public static UIBangunRuang Instance { get; private set; }

    [Header("Panel Referensi")]
    public GameObject panelInfo;
    public GameObject panelTombol;

    [Header("Komponen Teks")]
    public TextMeshProUGUI teksJudul;
    public TextMeshProUGUI teksIsi;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SembunyikanSemua();
    }

    private string GetNamaBangun(JenisBangunRuang jenis)
    {
        switch (jenis)
        {
            case JenisBangunRuang.Kubus: return "Kubus";
            case JenisBangunRuang.Balok: return "Balok";
            case JenisBangunRuang.Bola: return "Bola";
            case JenisBangunRuang.Kerucut: return "Kerucut";
            case JenisBangunRuang.Tabung: return "Tabung";
            case JenisBangunRuang.PrismaSegitiga: return "PrismaSegitiga";
            case JenisBangunRuang.LimasSegitiga: return "LimasSegitiga";
            case JenisBangunRuang.LimasSegiempat: return "LimasSegiempat";
            case JenisBangunRuang.LimasSegilima: return "LimasSegilima";
            case JenisBangunRuang.LimasSegienam: return "LimasSegienam";
            default: return jenis.ToString();
        }
    }

    // ========= KONTROL PANEL =========
    public void TampilTombol(bool tampil)
    {
        if (panelTombol != null)
            panelTombol.SetActive(tampil);
    }

    public void SembunyikanSemua()
    {
        if (panelInfo != null) panelInfo.SetActive(false);
        if (panelTombol != null) panelTombol.SetActive(false);

        if (teksJudul != null) teksJudul.text = "";
        if (teksIsi != null) teksIsi.text = "";
    }

    // ========= KONTEN TEKS: INFO =========
    public void TampilInfo(JenisBangunRuang jenis)
    {
        if (panelInfo != null) panelInfo.SetActive(true);

        string nama = GetNamaBangun(jenis);
        teksJudul.text = "Info " + nama;

        switch (jenis)
        {
            case JenisBangunRuang.Kubus:
                teksIsi.text = "Nama model: " + nama +
                               "\nKubus memiliki 6 sisi persegi yang sama luas, 12 rusuk sama panjang, dan 8 titik sudut.";
                break;

            case JenisBangunRuang.Balok:
                teksIsi.text = "Nama model: " + nama +
                               "\nBalok memiliki 6 sisi (3 pasang sisi sejajar), 12 rusuk, dan 8 titik sudut.";
                break;

            case JenisBangunRuang.PrismaSegitiga:
                teksIsi.text = "Nama model: " + nama +
                               "\nPrisma segitiga dibatasi oleh dua segitiga sejajar (alas dan tutup) dan 3 sisi tegak berbentuk persegi panjang.";
                break;

            case JenisBangunRuang.Tabung:
                teksIsi.text = "Nama model: " + nama +
                               "\nTabung memiliki 2 sisi datar berbentuk lingkaran (alas dan tutup) dan 1 sisi lengkung (selimut).";
                break;

            case JenisBangunRuang.Kerucut:
                teksIsi.text = "Nama model: " + nama +
                               "\nKerucut memiliki 1 alas lingkaran, 1 sisi lengkung, dan 1 titik puncak.";
                break;

            case JenisBangunRuang.Bola:
                teksIsi.text = "Nama model: " + nama +
                               "\nBola memiliki 1 sisi lengkung dan tidak memiliki rusuk ataupun titik sudut.";
                break;

            case JenisBangunRuang.LimasSegitiga:
                teksIsi.text = "Nama model: " + nama +
                               "\nLimas segitiga memiliki alas segitiga dan 3 sisi tegak segitiga yang bertemu di satu titik puncak.";
                break;

            case JenisBangunRuang.LimasSegiempat:
                teksIsi.text = "Nama model: " + nama +
                               "\nLimas segiempat memiliki alas segiempat dan 4 sisi tegak segitiga yang bertemu di satu titik puncak.";
                break;

            case JenisBangunRuang.LimasSegilima:
                teksIsi.text = "Nama model: " + nama +
                               "\nLimas segilima memiliki alas segilima dan 5 sisi tegak segitiga yang bertemu di satu titik puncak.";
                break;

            case JenisBangunRuang.LimasSegienam:
                teksIsi.text = "Nama model: " + nama +
                               "\nLimas segienam memiliki alas segienam dan 6 sisi tegak segitiga yang bertemu di satu titik puncak.";
                break;

            default:
                teksIsi.text = "Nama model: " + nama + "\nInformasi belum tersedia.";
                break;
        }
    }

    // ========= KONTEN TEKS: RUMUS =========
    public void TampilRumus(JenisBangunRuang jenis)
    {
        if (panelInfo != null) panelInfo.SetActive(true);

        string nama = GetNamaBangun(jenis);
        teksJudul.text = "Rumus " + nama;


        switch (jenis)
        {
            case JenisBangunRuang.Kubus:
                teksIsi.text = "Volume = s × s × s\n" +
                               "Luas = 6 × s × s\n\n" +
                               "<b>Keterangan:</b>\n" +
                               "• s = Panjang sisi (rusuk)";
                break;

            case JenisBangunRuang.Balok:
                teksIsi.text = "Volume = p × l × t\n" +
                               "Luas = 2 × ((p × l) + (p × t) + (l × t))\n\n" +
                               "<b>Keterangan:</b>\n" +
                               "• p = Panjang\n" +
                               "• l = Lebar\n" +
                               "• t = Tinggi";
                break;

            case JenisBangunRuang.PrismaSegitiga:
                teksIsi.text = "Volume = (1/2 × a × t_alas) × t_prisma\n" +
                               "Luas = (2 × Luas alas) + (Keliling alas × t_prisma)\n\n" +
                               "<b>Keterangan:</b>\n" +
                               "• a = Alas segitiga\n" +
                               "• t_alas = Tinggi segitiga\n" +
                               "• t_prisma = Tinggi bangun prisma";
                break;

            case JenisBangunRuang.Tabung:
                teksIsi.text = "Volume = π × r² × t\n" +
                               "Luas = 2 × π × r × (r + t)\n\n" +
                               "<b>Keterangan:</b>\n" +
                               "• r = Jari-jari lingkaran alas\n" +
                               "• t = Tinggi tabung\n" +
                               "• π = 22/7 atau 3.14";
                break;

            case JenisBangunRuang.Kerucut:
                teksIsi.text = "Volume = 1/3 × π × r² × t\n" +
                               "Luas = π × r × (r + s)\n\n" +
                               "<b>Keterangan:</b>\n" +
                               "• r = Jari-jari alas\n" +
                               "• t = Tinggi kerucut\n" +
                               "• s = Garis pelukis (sisi miring)";
                break;

            case JenisBangunRuang.Bola:
                teksIsi.text = "Volume = 4/3 × π × r³\n" +
                               "Luas = 4 × π × r²\n\n" +
                               "<b>Keterangan:</b>\n" +
                               "• r = Jari-jari bola\n" +
                               "• π = 22/7 atau 3.14";
                break;

            case JenisBangunRuang.LimasSegitiga:
            case JenisBangunRuang.LimasSegiempat:
            case JenisBangunRuang.LimasSegilima:
            case JenisBangunRuang.LimasSegienam:
                teksIsi.text = "Volume = 1/3 × Luas Alas × t\n" +
                               "Luas = Luas Alas + Jumlah Luas Sisi Tegak\n\n" +
                               "<b>Keterangan:</b>\n" +
                               "• t = Tinggi limas (dari puncak ke alas)\n" +
                               "• Luas Alas = Tergantung bentuk alasnya";
                break;
        }
    }

    // ========= KONTEN TEKS: RUSUK =========
    public void TampilRusuk(JenisBangunRuang jenis)
    {
        if (panelInfo != null) panelInfo.SetActive(true);

        string nama = GetNamaBangun(jenis);
        teksJudul.text = "Rusuk " + nama;

        switch (jenis)
        {
            case JenisBangunRuang.Tabung:
                teksIsi.text = "Tabung tidak memiliki rusuk lurus, hanya dua tepi lingkaran pada alas dan tutup.";
                break;

            case JenisBangunRuang.Kerucut:
                teksIsi.text = "Kerucut memiliki satu tepi lingkaran pada alas.";
                break;

            case JenisBangunRuang.Bola:
                teksIsi.text = "Bola tidak memiliki rusuk.";
                break;

            case JenisBangunRuang.PrismaSegitiga:
                teksIsi.text = "Prisma segitiga memiliki 9 rusuk: 3 rusuk di alas, 3 rusuk di tutup, dan 3 rusuk tegak.";
                break;

            case JenisBangunRuang.LimasSegitiga:
                teksIsi.text = "Limas segitiga memiliki 6 rusuk: 3 rusuk alas dan 3 rusuk yang menuju puncak.";
                break;

            case JenisBangunRuang.LimasSegiempat:
                teksIsi.text = "Limas segiempat memiliki 8 rusuk: 4 rusuk alas dan 4 rusuk yang menuju puncak.";
                break;

            case JenisBangunRuang.LimasSegilima:
                teksIsi.text = "Limas segilima memiliki 10 rusuk: 5 rusuk alas dan 5 rusuk yang menuju puncak.";
                break;

            case JenisBangunRuang.LimasSegienam:
                teksIsi.text = "Limas segienam memiliki 12 rusuk: 6 rusuk alas dan 6 rusuk yang menuju puncak.";
                break;

            default:
                teksIsi.text = "Perhatikan garis-garis (kerangka) bangun ini untuk melihat rusuk-rusuknya.";
                break;
        }
    }

    // ========= KONTEN TEKS: JARING =========
    public void TampilJaring(JenisBangunRuang jenis)
    {
        if (panelInfo != null) panelInfo.SetActive(true);

        string nama = GetNamaBangun(jenis);
        teksJudul.text = "Jaring-jaring " + nama;
        teksIsi.text = "Perhatikan animasi jaring-jaring yang melipat membentuk bangun ruang " + nama + ".";
    }
}