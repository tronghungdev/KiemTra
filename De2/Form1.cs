using BLL.De2;
using DAL.De2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace De2
{
    public partial class Form1 : Form
    {
        private SanphamService sanphamService;
        private bool isEdited = false;

        public Form1()
        {
            InitializeComponent();
            sanphamService = new SanphamService();
            LoadDataToDataGridView();
            LoadLoaiSPToComboBox();
            SetButtonsEnabled(false);
        }
        private void SetButtonsEnabled(bool enabled)
        {
            btnSave.Enabled = enabled;
            btnNoSave.Enabled = enabled;
        }
        private bool ShowConfirmationDialog(string message)
        {
            var result = MessageBox.Show(message, "Xác nhận thay đổi.", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return result == DialogResult.Yes;
        }
        private void LoadLoaiSPToComboBox()
        {
            List<LoaiSP> loaiSPs = sanphamService.GetAllLoaiSP(); 
            cmbLoai.DataSource = loaiSPs;
            cmbLoai.DisplayMember = "TenLoai";
            cmbLoai.ValueMember = "MaLoai"; 
        }

        private void LoadDataToDataGridView(string searchTerm = "")
        {
             var sanphams = sanphamService.getAll()
        .Where(s => string.IsNullOrEmpty(searchTerm) || s.TenSP.Contains(searchTerm))
        .Select(s => new
        {
            s.MaSP,
            s.TenSP,
            s.Ngaynhap,
            MaLoai = s.MaLoai,
            LoaiSP = s.LoaiSP != null ? s.LoaiSP.TenLoai : "" 
        }).ToList();
    
    dataGridView1.DataSource = sanphams;

    if (!sanphams.Any())
    {
        MessageBox.Show("Không tìm thấy sản phẩm nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMSP.Text) || string.IsNullOrEmpty(txtTenSP.Text) || cmbLoai.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (ShowConfirmationDialog("Bạn có chắc chắn muốn thêm sản phẩm này không?"))
            {
                try
                {
                    Sanpham sanpham = new Sanpham
                    {
                        MaSP = txtMSP.Text,
                        TenSP = txtTenSP.Text,
                        Ngaynhap = dtpNgayNhap.Value,
                        MaLoai = cmbLoai.SelectedValue.ToString()
                    };

                    sanphamService.AddSanpham(sanpham);
                    LoadDataToDataGridView();
                    ClearFields();
                    SetButtonsEnabled(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}\n{ex.InnerException?.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMSP.Text))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string currentMaSP = dataGridView1.CurrentRow.Cells["MaSP"].Value.ToString();
            if (txtMSP.Text != currentMaSP)
            {
                MessageBox.Show("Mã sản phẩm không thể sửa. Vui lòng xóa sản phẩm này và thêm mã sản phẩm mới.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var result = MessageBox.Show("Bạn có chắc chắn muốn sửa sản phẩm này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                return; 
            }

            try
            {
                Sanpham sanpham = new Sanpham
                {
                    MaSP = currentMaSP,
                    TenSP = txtTenSP.Text,
                    Ngaynhap = dtpNgayNhap.Value,
                    MaLoai = cmbLoai.SelectedValue.ToString()
                };

                sanphamService.UpdateSanpham(sanpham);
                LoadDataToDataGridView();
                SetButtonsEnabled(true);
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMSP.Text))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }
            if (ShowConfirmationDialog("Bạn có chắc chắn muốn xóa sản phẩm này không?"))
            {
                string maSP = txtMSP.Text;
                sanphamService.DeleteSanpham(maSP);
                LoadDataToDataGridView();
                SetButtonsEnabled(true);
                ClearFields();
            }
        }
       
        private void ClearFields()
        {
            txtMSP.Clear();
            txtTenSP.Clear();
            dtpNgayNhap.Value = DateTime.Now; 
            cmbLoai.SelectedIndex = -1; 
            isEdited = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];

 
                txtMSP.Text = row.Cells["MaSP"].Value.ToString();
                txtTenSP.Text = row.Cells["TenSP"].Value.ToString();
                dtpNgayNhap.Value = Convert.ToDateTime(row.Cells["Ngaynhap"].Value);
                cmbLoai.SelectedValue = row.Cells["MaLoai"].Value;
                isEdited = true; 
                SetButtonsEnabled(false); 
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string searchTerm = txtTimKiem.Text.Trim(); 
            LoadDataToDataGridView(searchTerm);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn lưu tất cả dữ liệu không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue; 

                        Sanpham sanpham = new Sanpham
                        {
                            MaSP = row.Cells["MaSP"].Value.ToString(),
                            TenSP = row.Cells["TenSP"].Value.ToString(),
                            Ngaynhap = Convert.ToDateTime(row.Cells["Ngaynhap"].Value),
                            MaLoai = row.Cells["MaLoai"].Value.ToString()
                        };
                        if (IsProductExists(sanpham.MaSP)) 
                        {
                            sanphamService.UpdateSanpham(sanpham);
                        }
                        else
                        {
                            sanphamService.AddSanpham(sanpham);
                        }
                    }

                    MessageBox.Show("Dữ liệu đã được lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataToDataGridView();
                    SetButtonsEnabled(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private bool IsProductExists(string maSP)
        {
            return sanphamService.getAll().Any(s => s.MaSP == maSP);
        }

        private void btnNoSave_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn không muốn lưu không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ClearFields();
                SetButtonsEnabled(false);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (ShowConfirmationDialog("Bạn có chắc chắn muốn đóng ứng dụng không?"))
            {
                Close();
            }
        }
    }
}
