import pandas as pd
import numpy as np

# Thiết lập thông số
n_users = 200
n_items = 50
n_ratings = 2000 # Số lượng đánh giá thực tế (sparse)

# Tạo dữ liệu ngẫu nhiên
np.random.seed(42)
users = np.random.randint(0, n_users, n_ratings)
items = np.random.randint(0, n_items, n_ratings)

# Tạo rating cho 4 tiêu chí (thang điểm 1-5)
# Chúng ta giả định các tiêu chí có sự tương quan nhẹ với nhau
c1 = np.random.randint(1, 6, n_ratings)
c2 = np.clip(c1 + np.random.randint(-1, 2, n_ratings), 1, 5)
c3 = np.random.randint(1, 6, n_ratings)
c4 = np.clip(c3 + np.random.randint(-1, 2, n_ratings), 1, 5)

# Đánh giá tổng thể thường là trung bình của các tiêu chí + một chút nhiễu
overall = np.round((c1 + c2 + c3 + c4) / 4).astype(int)

# Tạo DataFrame
df = pd.DataFrame({
    'user_id': users,
    'item_id': items,
    'service': c1,
    'cleanliness': c2,
    'value': c3,
    'location': c4,
    'overall_rating': overall
})

# Loại bỏ các cặp (user, item) trùng lặp
df = df.drop_duplicates(subset=['user_id', 'item_id'])

# Lưu thành file
df.to_csv('multicriteria_data.csv', index=False)
print("Đã tạo file 'multicriteria_data.csv' thành công!")