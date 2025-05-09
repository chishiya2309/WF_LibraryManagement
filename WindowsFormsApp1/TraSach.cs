﻿using BusinessAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class TraSach: Form
    {
        DBLoanAndReturn dBLoanAndReturn;
        DataTable dtLAR;
        public TraSach()
        {
            InitializeComponent();
            dBLoanAndReturn = new DBLoanAndReturn();
            LoadData();
        }

        public void LoadData()
        {
            dtLAR = new DataTable();
            dtLAR.Clear();
            dtLAR = dBLoanAndReturn.LayPhieuMuon().Tables[0];

            cmbMaPhieu.DataSource = dtLAR;
            cmbMaPhieu.DisplayMember = "MaPhieu";
            cmbMaPhieu.ValueMember = "MaPhieu";

            cmbMaPhieu.SelectedIndex = 0;

            dtpNgayTra.Value = DateTime.Now;
            lblNgayTraInfo.Text = "Lưu ý: Ngày trả thực tế sẽ được ghi nhận khi bạn ấn nút 'Lưu'";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                int maPhieuMuon = (int)cmbMaPhieu.SelectedValue;
                DateTime ngayTra = dtpNgayTra.Value;

                string err = "";
                bool success = dBLoanAndReturn.TraSach(ref err, maPhieuMuon, ngayTra);

                if (success)
                {
                    MessageBox.Show($"Trả sách thành công!\nNgày trả thực tế đã được ghi nhận: {ngayTra.ToShortDateString()}",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Lỗi khi trả sách: " + err, "Lỗi",
                         MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TraSach_Load(object sender, EventArgs e)
        {

        }

        private void cmbMaPhieu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbMaPhieu.SelectedValue != null)
            {
                try
                {
                    // Lấy phiếu mượn đã chọn
                    int maPhieu = (int)cmbMaPhieu.SelectedValue;
                    DataRow[] rows = dtLAR.Select($"MaPhieu = {maPhieu}");

                    if (rows.Length > 0)
                    {
                        // Hiển thị thông tin hạn trả của phiếu mượn
                        DateTime hanTra = Convert.ToDateTime(rows[0]["HanTra"]);
                        string trangThai = rows[0]["TrangThai"].ToString();

                        lblPhieuMuonInfo.Text = $"Hạn trả: {hanTra.ToShortDateString()}, Trạng thái: {trangThai}";

                        // Kiểm tra nếu quá hạn
                        if (DateTime.Now > hanTra && trangThai == "Quá hạn")
                        {
                            lblPhieuMuonInfo.ForeColor = Color.Red;
                        }
                        else
                        {
                            lblPhieuMuonInfo.ForeColor = Color.Black;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblPhieuMuonInfo.Text = "";
                }
            }    
        }
    }
}
