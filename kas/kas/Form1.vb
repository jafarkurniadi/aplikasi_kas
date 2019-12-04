Imports System.Data.Odbc
Public Class Form1
    Public tampil As Integer
    Public saldo As Integer

    Sub TampilGrid()
        bukakoneksi()

        DA = New OdbcDataAdapter("select * from tbl_kas", CONN)
        DS = New DataSet
        DA.Fill(DS, "tbl_kas")
        DataGridView1.DataSource = DS.Tables("tbl_kas")

        tutupkoneksi()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TampilGrid()

        ComboJenis()
        updateDisplay()
    End Sub

    Sub ComboJenis()
        ComboBox1.Items.Add("Masuk")
        ComboBox1.Items.Add("Keluar")
    End Sub


    Sub KosongkanData()
        txtKode.Text = ""
        txtJumlah.Text = ""
        txtKet.Text = ""
        ComboBox1.Text = ""
    End Sub

    Private Sub BtnSimpan_Click(sender As Object, e As EventArgs) Handles btnSimpan.Click
        If txtJumlah.Text = "" Or txtKet.Text = "" Then
            MsgBox("Silahkan isi semua form")
        Else
            bukakoneksi()
            If ComboBox1.Text = "Masuk" Then
                saldo = CInt(txtJumlah.Text)
                tampil = tampil + saldo
            ElseIf ComboBox1.Text = "Keluar" Then
                saldo = CInt(txtJumlah.Text)
                tampil = tampil - saldo
            End If

            If tampil < 0 Then
                tampil = tampil + saldo
                MsgBox("Saldo anda tidak cukup")
            Else
                Dim simpan As String = "INSERT INTO tbl_kas (kd_transaksi,tanggal,jenis, jml_uang, keterangan, saldo) values('" & txtKode.Text & "','" & Format(DateTimePicker1.Value, "yyyy-MM-dd") & "', '" & ComboBox1.Text & "','" & txtJumlah.Text & "','" & txtKet.Text & "','" & tampil & "') "
                CMD = New OdbcCommand(simpan, CONN)
                CMD.ExecuteNonQuery()
                MsgBox("Data Berhasil di Input")
            End If

            updateDisplay()
            TampilGrid()
            KosongkanData()

            tutupkoneksi()
        End If
    End Sub

    Private Sub txtKode_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtKode.KeyPress
        txtKode.MaxLength = 6
        If e.KeyChar = Chr(13) Then
            bukakoneksi()
            CMD = New OdbcCommand("select * from tbl_kas where kd_transaksi= '" & txtKode.Text & "'", CONN)
            RD = CMD.ExecuteReader
            RD.Read()
            If Not RD.HasRows Then
                MsgBox("Kode transaksi tidak ada")
                txtKode.Focus()
            Else
                ComboBox1.Text = RD.Item("jenis")
                txtJumlah.Text = RD.Item("jml_uang")
                txtKet.Text = RD.Item("keterangan")
                DateTimePicker1.Focus()
            End If
        End If
    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        bukakoneksi()
        Dim edit As String = "update tbl_kas set
            tanggal = '" & Format(DateTimePicker1.Value, "yyyy-MM-dd") & "',
            jenis = '" & ComboBox1.Text & "',
            jml_uang = '" & txtJumlah.Text & "',
            keterangan = '" & txtKet.Text & "'
            where kd_transaksi = '" & txtKode.Text & "'"

        CMD = New OdbcCommand(edit, CONN)
        CMD.ExecuteNonQuery()
        MsgBox("data berhasil diupdate")
        TampilGrid()
        updateDisplay()
        KosongkanData()
        tutupkoneksi()
    End Sub

    Private Sub BtnHapus_Click(sender As Object, e As EventArgs) Handles btnHapus.Click
        If txtKode.Text = "" Then
            MsgBox("Silahkan Pilih Data yang akan dihapus dengan memasukkan kode transaksi dan tekan ENTER")
        Else
            If MessageBox.Show("Yakin akan dihapus ?", "", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                bukakoneksi()
                Dim hapus As String = "delete From tbl_kas where kd_transaksi= '" & txtKode.Text & "' "
                CMD = New OdbcCommand(hapus, CONN)
                CMD.ExecuteNonQuery()
                updateDisplay()
                TampilGrid()
                KosongkanData()
                tutupkoneksi()
            End If
        End If
    End Sub

    Private Sub updateDisplay()
        bukakoneksi()
        CMD = New OdbcCommand(" select * from tbl_kas order by kd_transaksi desc limit 1", CONN)
        RD = CMD.ExecuteReader
        RD.Read()
        If Not RD.HasRows Then
            lbl_Saldo.Text = " Saldo Utama Rp. 0" & RD.Item("saldo")
        Else
            lbl_Saldo.Text = " Saldo Utama Rp. " & RD.Item("saldo")
            tampil = RD.Item("saldo")
        End If
        tutupkoneksi()
    End Sub
End Class