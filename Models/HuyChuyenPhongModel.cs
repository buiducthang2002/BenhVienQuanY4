public class HuyChuyenPhongModel
{
    public long makcb { get; set; }              // ĐỔI từ string → long
    public string hoten { get; set; }
    public string madieutri { get; set; }
    
    public int? makk { get; set; }            // Mã phòng gốc
    public string tenphonggoc { get; set; }   // Tên phòng gốc
    
    public int? makkc { get; set; }           // Mã phòng đã chuyển
    public string tenphongchuyen { get; set; } // Tên phòng đã chuyển
    
    public DateTime? ngaychuyen { get; set; }  // Ngày chuyển phòng
    public string tinhtrang { get; set; }      // Trạng thái tiếp nhận

}
