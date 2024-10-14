using DAL.De2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.De2
{
    public class SanphamService
    {
        public readonly QLSanPhamModel _context;


        public SanphamService()
        {
            _context = new QLSanPhamModel();
        }

        public List<Sanpham> getAll()
        {
            return _context.Sanphams.ToList();
        }
        public void AddSanpham(Sanpham sanpham)
        {
            _context.Sanphams.Add(sanpham);
            _context.SaveChanges();
        }

        public void UpdateSanpham(Sanpham sanpham)
        {
            var existingSanpham = _context.Sanphams.Find(sanpham.MaSP);
            if (existingSanpham != null)
            {
                existingSanpham.TenSP = sanpham.TenSP;
                existingSanpham.Ngaynhap = sanpham.Ngaynhap;
                existingSanpham.MaLoai = sanpham.MaLoai;

                _context.SaveChanges();
            }
        }

        public void DeleteSanpham(string maSP)
        {
            var sanpham = _context.Sanphams.Find(maSP);
            if (sanpham != null)
            {
                _context.Sanphams.Remove(sanpham);
                _context.SaveChanges();
            }
        }
        public List<LoaiSP> GetAllLoaiSP()
        {
            return _context.LoaiSPs.ToList();
        }
    }
}
