-- TeknoSeedDataFull.sql
-- Full seed for Tekno (PostgreSQL)
-- Place: Tekno.Infrastructure/Persistence/Seed/TeknoSeedDataFull.sql
-- Run: psql -U postgres -d tekno_db -f TeknoSeedDataFull.sql
-- or paste into pgAdmin4 Query Tool and Run

BEGIN;

-- ===========================
-- BRANDS
-- ===========================
INSERT INTO brand (id, name, slug, country, logourl, createdat, updatedat) VALUES
(1, 'Dell', 'dell', 'USA', 'https://worldvectorlogo.com/logo/dell-2', NOW(), NOW()),
(2, 'Apple', 'apple', 'USA', 'https://worldvectorlogo.com/logo/apple-13', NOW(), NOW()),
(3, 'Asus', 'asus', 'Taiwan', 'https://worldvectorlogo.com/logo/asus-4', NOW(), NOW()),
(4, 'HP', 'hp', 'USA', 'https://worldvectorlogo.com/logo/HP-5', NOW(), NOW()),
(5, 'Lenovo', 'lenovo', 'China', 'https://worldvectorlogo.com/logo/lenovo-2', NOW(), NOW()),
(6, 'Samsung', 'samsung', 'Korea', 'https://worldvectorlogo.com/logo/samsung-8', NOW(), NOW()),
(7, 'Google', 'google', 'USA', 'https://worldvectorlogo.com/logo/google-1', NOW(), NOW()),
(8, 'Xiaomi', 'xiaomi', 'China', 'https://worldvectorlogo.com/logo/xiaomi-5', NOW(), NOW()),
(9, 'OnePlus', 'oneplus', 'China', 'https://worldvectorlogo.com/logo/oneplus-2', NOW(), NOW()),
(10, 'LG', 'lg', 'Korea', 'https://worldvectorlogo.com/logo/lg', NOW(), NOW()),
(11, 'Logitech', 'logitech', 'Switzerland', 'https://worldvectorlogo.com/logo/logitech-gaming-2', NOW(), NOW()),
(12, 'Razer', 'razer', 'USA', 'https://worldvectorlogo.com/logo/razer-1', NOW(), NOW()),
(13, 'Sony', 'sony', 'Japan', 'https://worldvectorlogo.com/logo/sony-2', NOW(), NOW()),
(14, 'Anker', 'anker', 'China', 'https://worldvectorlogo.com/logo/anker-logo-1', NOW(), NOW()),
(15, 'Baseus', 'baseus', 'China', 'https://mms.img.susercontent.com/vn-11134216-7r98o-lnicyi57m5x6fd', NOW(), NOW()),
(16, 'Spigen', 'spigen', 'USA', 'https://spigen.vn/wp-content/uploads/2023/09/Spigen_Header_New_Logo.png', NOW(), NOW()),
(17, 'UAG', 'uag', 'USA', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRlpSaYkZMxWktmvmvOx7mDurTEDu0KXqz1HQ&s', NOW(), NOW())
ON CONFLICT (id) DO NOTHING;

-- ===========================
-- CATEGORIES
-- ===========================
INSERT INTO category (id, name, slug, description, parentid, createdat, updatedat) VALUES
(1, 'Laptop', 'laptop', 'All kinds of laptops', NULL, NOW(), NOW()),
(2, 'Smartphone', 'smartphone', 'All kinds of smartphones', NULL, NOW(), NOW()),
(3, 'Tablet', 'tablet', 'All kinds of tablets', NULL, NOW(), NOW()),
(4, 'Accessory', 'accessory', 'External product that enhances main product experience', NULL, NOW(), NOW()),
(5, 'Camera', 'camera', 'All kinds of cameras', NULL, NOW(), NOW()),
(6, 'Computer & Office', 'computer-office', 'PC and office related products', NULL, NOW(), NOW()),
(7, 'Gaming', 'gaming', 'Gaming products and accessories', NULL, NOW(), NOW()),
(8, 'Monitor', 'monitor', 'All types of computer monitors', 6, NOW(), NOW()),
(9, 'CPU', 'cpu', 'Processors and chips for computers', 6, NOW(), NOW()),
(10, 'GPU', 'gpu', 'Graphics cards for PCs and laptops', 6, NOW(), NOW()),
(11, 'RAM', 'ram', 'Memory modules for PCs and laptops', 6, NOW(), NOW()),
(12, 'Storage (SSD / HDD)', 'storage', 'Storage devices: SSD, HDD, memory cards', 6, NOW(), NOW()),
(13, 'Keyboard', 'keyboard', 'Keyboards for PC, Laptop, and Tablet', 4, NOW(), NOW()),
(14, 'Mouse', 'mouse', 'Computer and laptop mice (wired, wireless, gaming)', 4, NOW(), NOW()),
(15, 'Headphone / Headset', 'headphone', 'Audio accessories compatible with PC, Laptop, and Smartphone', 4, NOW(), NOW()),
(16, 'Charger & Cable', 'charger-cable', 'Chargers, adapters, and data cables for all devices', 4, NOW(), NOW()),
(17, 'Case & Cover', 'case-cover', 'Protective cases for phones, tablets, and laptops', 4, NOW(), NOW())
ON CONFLICT (id) DO NOTHING;

-- ===========================
-- PRODUCT ATTRIBUTES
-- ===========================
INSERT INTO product_attribute (id, name, inputtype, isglobal, categoryid, createdat, updatedat) VALUES
(1, 'Color', 'select', true, NULL, NOW(), NOW()),
(2, 'Warranty Period', 'number', true, NULL, NOW(), NOW()),
(10, 'Screen Size', 'select', false, 1, NOW(), NOW()),
(11, 'CPU', 'select', false, 1, NOW(), NOW()),
(12, 'RAM', 'select', false, 1, NOW(), NOW()),
(13, 'Storage', 'select', false, 1, NOW(), NOW()),
(14, 'GPU', 'select', false, 1, NOW(), NOW()),
(20, 'Screen Size', 'select', false, 2, NOW(), NOW()),
(21, 'Battery Capacity', 'number', false, 2, NOW(), NOW()),
(22, 'Camera Resolution', 'select', false, 2, NOW(), NOW()),
(23, 'RAM', 'select', false, 2, NOW(), NOW()),
(24, 'Storage', 'select', false, 2, NOW(), NOW()),
(30, 'Screen Size', 'select', false, 3, NOW(), NOW()),
(31, 'Battery Capacity', 'number', false, 3, NOW(), NOW()),
(32, 'RAM', 'select', false, 3, NOW(), NOW()),
(33, 'Storage', 'select', false, 3, NOW(), NOW()),
(40, 'Processor Type', 'select', false, 6, NOW(), NOW()),
(41, 'RAM Type', 'select', false, 6, NOW(), NOW()),
(42, 'GPU Model', 'select', false, 6, NOW(), NOW()),
(50, 'Screen Size', 'select', false, 8, NOW(), NOW()),
(51, 'Refresh Rate', 'select', false, 8, NOW(), NOW()),
(52, 'Resolution', 'select', false, 8, NOW(), NOW()),
(53, 'Panel Type', 'select', false, 8, NOW(), NOW()),
(60, 'Compatibility', 'select', false, 4, NOW(), NOW()),
(61, 'Connection Type', 'select', false, 4, NOW(), NOW()),
(70, 'Switch Type', 'select', false, 13, NOW(), NOW()),
(71, 'Backlight', 'select', false, 13, NOW(), NOW()),
(72, 'Layout', 'select', false, 13, NOW(), NOW()),
(73, 'Connection Type', 'select', false, 13, NOW(), NOW()),
(80, 'DPI', 'number', false, 14, NOW(), NOW()),
(81, 'Connection Type', 'select', false, 14, NOW(), NOW()),
(82, 'RGB Lighting', 'select', false, 14, NOW(), NOW()),
(90, 'Type', 'select', false, 15, NOW(), NOW()),
(91, 'Connection Type', 'select', false, 15, NOW(), NOW()),
(92, 'Has Microphone', 'select', false, 15, NOW(), NOW()),
(100, 'Connector Type', 'select', false, 16, NOW(), NOW()),
(101, 'Power Output (W)', 'number', false, 16, NOW(), NOW()),
(102, 'Cable Length (m)', 'number', false, 16, NOW(), NOW()),
(110, 'Material', 'select', false, 17, NOW(), NOW()),
(111, 'Device Type', 'select', false, 17, NOW(), NOW()),
(112, 'Shock Resistant', 'select', false, 17, NOW(), NOW())
ON CONFLICT (id) DO NOTHING;

-- ===========================
-- ATTRIBUTE VALUES
-- ===========================
INSERT INTO attribute_value (id, attributeid, value, createdat, updatedat) VALUES
(1, 1, 'Black', NOW(), NOW()),
(2, 1, 'White', NOW(), NOW()),
(3, 1, 'Silver', NOW(), NOW()),
(4, 1, 'Blue', NOW(), NOW()),
(5, 1, 'Red', NOW(), NOW()),
(10, 2, 'Apple', NOW(), NOW()),
(11, 2, 'Samsung', NOW(), NOW()),
(12, 2, 'Asus', NOW(), NOW()),
(13, 2, 'HP', NOW(), NOW()),
(14, 2, 'Dell', NOW(), NOW()),
(20, 11, 'Intel i5', NOW(), NOW()),
(21, 11, 'Intel i7', NOW(), NOW()),
(22, 11, 'AMD Ryzen 5', NOW(), NOW()),
(23, 11, 'AMD Ryzen 7', NOW(), NOW()),
(30, 12, '8GB', NOW(), NOW()),
(31, 12, '16GB', NOW(), NOW()),
(32, 12, '32GB', NOW(), NOW()),
(40, 13, '256GB SSD', NOW(), NOW()),
(41, 13, '512GB SSD', NOW(), NOW()),
(42, 13, '1TB SSD', NOW(), NOW()),
(50, 10, '13 inch', NOW(), NOW()),
(51, 10, '15 inch', NOW(), NOW()),
(52, 10, '17 inch', NOW(), NOW()),
(60, 14, 'RTX 4060', NOW(), NOW()),
(61, 14, 'RTX 4070', NOW(), NOW()),
(62, 14, 'GTX 1650', NOW(), NOW()),
(70, 70, 'Red', NOW(), NOW()),
(71, 70, 'Blue', NOW(), NOW()),
(72, 70, 'Brown', NOW(), NOW()),
(80, 61, 'Wired', NOW(), NOW()),
(81, 61, 'Wireless', NOW(), NOW()),
(82, 73, 'Wired', NOW(), NOW()),
(83, 81, 'Bluetooth', NOW(), NOW()),
(84, 91, '3.5mm', NOW(), NOW()),
(85, 91, 'USB-C', NOW(), NOW()),
(100, 110, 'Silicone', NOW(), NOW()),
(101, 110, 'Leather', NOW(), NOW()),
(102, 110, 'Plastic', NOW(), NOW())
ON CONFLICT (id) DO NOTHING;

-- ===========================
-- PRODUCTS (full as provided)
-- ===========================
INSERT INTO product (id, name, slug, brandid, categoryid, baseprice, status, overview, description, createdat, updatedat) VALUES
-- Laptops
(1, 'Dell XPS 13', 'dell-xps-13', 1, 1, 1699.00, 'available', 'Premium ultrabook with compact design.', 'Dell XPS 13 series laptops designed for professionals.', NOW(), NOW()),
(2, 'MacBook Air', 'macbook-air', 2, 1, 1199.00, 'available', 'Ultra-thin and lightweight laptop by Apple.', 'MacBook Air powered by Apple M-series chips.', NOW(), NOW()),
(3, 'Asus ZenBook', 'asus-zenbook', 3, 1, 1450.00, 'available', 'Portable productivity ultrabook.', 'ZenBook series with Intel and AMD variants.', NOW(), NOW()),
(4, 'HP Spectre x360', 'hp-spectre-x360', 4, 1, 1799.00, 'available', 'Convertible premium laptop.', '2-in-1 design with touch and pen support.', NOW(), NOW()),
(5, 'Lenovo ThinkPad X1 Carbon', 'thinkpad-x1-carbon', 5, 1, 1999.00, 'available', 'Business ultrabook with robust build.', 'ThinkPad X1 Carbon for professionals.', NOW(), NOW()),

-- Smartphones
(10, 'iPhone 15 Pro', 'iphone-15-pro', 2, 2, 999.00, 'available', 'Apple flagship smartphone.', 'iPhone 15 Pro with titanium frame and A17 Pro chip.', NOW(), NOW()),
(11, 'Samsung Galaxy S24', 'galaxy-s24', 6, 2, 899.00, 'available', 'Next-gen Android flagship.', 'Galaxy S24 with AMOLED display and powerful camera.', NOW(), NOW()),
(12, 'Google Pixel 9', 'pixel-9', 7, 2, 799.00, 'available', 'Pure Android experience.', 'Pixel 9 with Tensor G4 chip and AI photography.', NOW(), NOW()),
(13, 'Xiaomi 14 Pro', 'xiaomi-14-pro', 8, 2, 850.00, 'available', 'High-end performance smartphone.', 'Xiaomi 14 Pro with Leica cameras.', NOW(), NOW()),
(14, 'OnePlus 12', 'oneplus-12', 9, 2, 749.00, 'available', 'Performance-focused smartphone.', 'OnePlus 12 offers fast charging and high refresh rate.', NOW(), NOW()),

-- Tablets
(20, 'iPad Pro', 'ipad-pro', 2, 3, 899.00, 'available', 'Powerful tablet for creators.', 'iPad Pro with M2 chip and ProMotion display.', NOW(), NOW()),
(21, 'Samsung Galaxy Tab S9', 'galaxy-tab-s9', 6, 3, 799.00, 'available', 'Android flagship tablet.', 'Galaxy Tab S9 series with AMOLED display.', NOW(), NOW()),
(22, 'Lenovo Tab P12', 'lenovo-tab-p12', 5, 3, 419.00, 'available', 'Affordable productivity tablet.', 'Tab P12 supports stylus and multi-tasking.', NOW(), NOW()),

-- Monitors
(30, 'Dell UltraSharp 27', 'dell-ultrasharp-27', 1, 8, 699.00, 'available', 'Professional 4K monitor.', 'Color-accurate UltraSharp series for designers.', NOW(), NOW()),
(31, 'LG Ultragear 32', 'lg-ultragear-32', 10, 8, 450.00, 'available', 'Gaming monitor with 165Hz refresh rate.', 'High-performance display for gamers.', NOW(), NOW()),

-- Keyboards
(40, 'Logitech MX Keys', 'logitech-mx-keys', 11, 13, 119.99, 'available', 'Wireless keyboard for professionals.', 'Backlit keyboard with multi-device support.', NOW(), NOW()),
(41, 'Razer BlackWidow V4', 'razer-blackwidow-v4', 12, 13, 169.99, 'available', 'Mechanical gaming keyboard.', 'RGB lighting and tactile feedback.', NOW(), NOW()),

-- Mouse
(50, 'Logitech MX Master 3S', 'logitech-mx-master-3s', 11, 14, 99.99, 'available', 'Ergonomic productivity mouse.', 'Supports multiple devices with customizable buttons.', NOW(), NOW()),
(51, 'Razer Viper V2 Pro', 'razer-viper-v2-pro', 12, 14, 149.99, 'available', 'Ultra-light gaming mouse.', 'Wireless mouse with high precision sensor.', NOW(), NOW()),

-- Headphones
(60, 'Sony WH-1000XM5', 'sony-wh-1000xm5', 13, 15, 399.99, 'available', 'Noise-cancelling wireless headphones.', 'Industry-leading noise cancellation for audio lovers.', NOW(), NOW()),
(61, 'Apple AirPods Pro 2', 'airpods-pro-2', 2, 15, 249.00, 'available', 'Wireless earbuds with active noise cancellation.', 'Compact design and adaptive sound control.', NOW(), NOW()),

-- Chargers & Cables
(70, 'Anker 65W GaN Charger', 'anker-65w-gan', 14, 16, 49.99, 'available', 'Fast charger with GaN technology.', 'Compact USB-C charger for laptops and phones.', NOW(), NOW()),
(71, 'Baseus USB-C Cable 1.5m', 'baseus-usb-c-cable', 15, 16, 14.99, 'available', 'Durable braided charging cable.', 'Supports fast charging and data transfer.', NOW(), NOW()),

-- Cases & Covers
(80, 'Spigen Rugged Armor Case', 'spigen-rugged-armor', 16, 17, 29.99, 'available', 'Protective phone case.', 'Shock-absorbing TPU material.', NOW(), NOW()),
(81, 'UAG Plasma Laptop Sleeve', 'uag-laptop-sleeve', 17, 17, 49.99, 'available', 'Durable protective sleeve.', 'Designed for 13–15 inch laptops.', NOW(), NOW())
ON CONFLICT (id) DO NOTHING;

-- ===========================
-- PRODUCT DETAIL (JSON specs) - product_detail table assumed columns: id, productid, specs (jsonb), createdat, updatedat
-- ===========================
INSERT INTO product_detail (productid, specs, createdat, updatedat) VALUES
(1, '{
  "Display": "13.4-inch FHD+ InfinityEdge",
  "CPU": "Intel Core i5 / i7",
  "RAM": "8GB / 16GB",
  "Storage": "512GB / 1TB SSD",
  "Weight": "1.2kg",
  "Battery": "52Wh",
  "OS": "Windows 11",
  "Warranty": "12 months"
}'::jsonb, NOW(), NOW()),
(2, '{
  "Display": "13.6-inch Liquid Retina",
  "Chip": "Apple M2",
  "RAM": "8GB / 16GB",
  "Storage": "256GB / 512GB",
  "Battery": "52.6Wh up to 18h",
  "OS": "macOS",
  "Weight": "1.24kg"
}'::jsonb, NOW(), NOW()),
(3, '{
  "Display": "14-inch OLED 2.8K",
  "CPU": "Intel i5 / i7 or Ryzen 7",
  "RAM": "8GB / 16GB",
  "Storage": "512GB / 1TB SSD",
  "OS": "Windows 11",
  "Weight": "1.3kg"
}'::jsonb, NOW(), NOW()),
(4, '{
  "Display": "13.5-inch 2-in-1 Touch OLED",
  "CPU": "Intel Core i5 / i7",
  "RAM": "8GB / 16GB",
  "Storage": "512GB / 1TB",
  "Convertible": true,
  "OS": "Windows 11 Home"
}'::jsonb, NOW(), NOW()),
(5, '{
  "Display": "14-inch IPS 2.8K",
  "CPU": "Intel i5 / i7",
  "RAM": "8GB / 16GB",
  "Storage": "512GB / 1TB",
  "Security": "Fingerprint + TPM 2.0",
  "OS": "Windows 11 Pro"
}'::jsonb, NOW(), NOW()),
(10, '{
  "Display": "6.1-inch OLED 120Hz",
  "Chip": "Apple A17 Pro",
  "RAM": "6GB",
  "Storage": "128GB / 256GB",
  "Camera": "48MP + 12MP + 12MP",
  "Battery": "3279mAh",
  "OS": "iOS 17"
}'::jsonb, NOW(), NOW()),
(11, '{
  "Display": "6.7-inch Dynamic AMOLED 120Hz",
  "Chip": "Snapdragon 8 Gen 3",
  "RAM": "8GB",
  "Storage": "128GB / 256GB",
  "Camera": "200MP + 12MP + 10MP",
  "Battery": "5000mAh",
  "OS": "Android 14"
}'::jsonb, NOW(), NOW()),
(12, '{
  "Display": "6.3-inch AMOLED 120Hz",
  "Chip": "Google Tensor G4",
  "RAM": "8GB",
  "Storage": "128GB / 256GB",
  "Camera": "50MP + 12MP",
  "Battery": "4700mAh",
  "OS": "Android 14"
}'::jsonb, NOW(), NOW()),
(13, '{
  "Display": "6.7-inch AMOLED QHD+",
  "Chip": "Snapdragon 8 Gen 3",
  "RAM": "12GB",
  "Storage": "256GB / 512GB",
  "Camera": "50MP + 50MP + 50MP (Leica)",
  "OS": "HyperOS (Android 14)"
}'::jsonb, NOW(), NOW()),
(14, '{
  "Display": "6.8-inch AMOLED 120Hz",
  "Chip": "Snapdragon 8 Gen 3",
  "RAM": "12GB",
  "Storage": "256GB / 512GB",
  "Battery": "5400mAh 100W charging",
  "OS": "OxygenOS 14"
}'::jsonb, NOW(), NOW()),
(20, '{
  "Display": "12.9-inch Liquid Retina XDR",
  "Chip": "Apple M2",
  "RAM": "8GB / 16GB",
  "Storage": "128GB / 256GB",
  "OS": "iPadOS 17",
  "PencilSupport": "Apple Pencil 2"
}'::jsonb, NOW(), NOW()),
(21, '{
  "Display": "11-inch AMOLED 120Hz",
  "Chip": "Snapdragon 8 Gen 2",
  "RAM": "8GB / 12GB",
  "Storage": "128GB / 256GB",
  "OS": "Android 14"
}'::jsonb, NOW(), NOW()),
(22, '{
  "Display": "12.7-inch LCD 144Hz",
  "Chip": "MediaTek Dimensity 7050",
  "RAM": "8GB",
  "Storage": "128GB",
  "Battery": "10200mAh",
  "OS": "Android 13"
}'::jsonb, NOW(), NOW()),
(30, '{
  "Display": "27-inch IPS 4K UHD",
  "Resolution": "3840x2160",
  "RefreshRate": "60Hz",
  "Ports": "HDMI, DisplayPort, USB-C",
  "ColorGamut": "99% sRGB",
  "Warranty": "24 months"
}'::jsonb, NOW(), NOW()),
(31, '{
  "Display": "32-inch VA QHD",
  "Resolution": "2560x1440",
  "RefreshRate": "165Hz",
  "Ports": "HDMI, DisplayPort",
  "Sync": "G-Sync Compatible"
}'::jsonb, NOW(), NOW()),
(40, '{ "Type": "Wireless", "Layout": "Full-size", "Backlight": "Yes", "Battery": "USB-C rechargeable" }'::jsonb, NOW(), NOW()),
(41, '{ "Type": "Mechanical", "Switch": "Razer Green", "Backlight": "RGB", "Connection": "Wired" }'::jsonb, NOW(), NOW()),
(50, '{ "Sensor": "Logitech Darkfield", "Connection": "Bluetooth / USB", "Battery": "70 days", "Buttons": 7 }'::jsonb, NOW(), NOW()),
(51, '{ "Sensor": "Focus Pro 30K", "Weight": "58g", "Connection": "Wireless", "Battery": "80h" }'::jsonb, NOW(), NOW()),
(60, '{ "Type": "Over-ear", "ANC": "Yes", "Battery": "30h", "Charging": "USB-C", "Microphone": "Yes" }'::jsonb, NOW(), NOW()),
(61, '{ "Type": "In-ear", "ANC": "Yes", "Battery": "6h + 24h", "Wireless": "Bluetooth 5.3" }'::jsonb, NOW(), NOW()),
(70, '{ "Power": "65W", "Ports": "2x USB-C, 1x USB-A", "Material": "GaN", "Input": "100–240V" }'::jsonb, NOW(), NOW()),
(71, '{ "Length": "1.5m", "Connector": "USB-C to USB-C", "Material": "Nylon braided", "MaxPower": "100W" }'::jsonb, NOW(), NOW()),
(80, '{ "Material": "TPU", "ShockResistant": "Yes", "CompatibleDevices": "iPhone 15" }'::jsonb, NOW(), NOW()),
(81, '{ "Material": "Ballistic Nylon", "Fits": "13–15 inch laptops", "WaterResistant": "Yes" }'::jsonb, NOW(), NOW())
ON CONFLICT (productid) DO NOTHING;

-- ===========================
-- PRODUCT VARIANTS
-- ===========================
INSERT INTO product_variant (id, productid, sku, price, stock, status, createdat, updatedat) VALUES
(1, 1, 'XPS13-I5-8-512', 1099.00, 20, 'available', NOW(), NOW()),
(2, 1, 'XPS13-I7-16-1TB', 1499.00, 10, 'available', NOW(), NOW()),
(3, 2, 'MBA-M2-8-256', 999.00, 15, 'available', NOW(), NOW()),
(4, 2, 'MBA-M2-16-512', 1299.00, 8, 'available', NOW(), NOW()),
(5, 3, 'ZEN-I5-8-512', 899.00, 25, 'available', NOW(), NOW()),
(6, 3, 'ZEN-I7-16-1TB', 1199.00, 12, 'available', NOW(), NOW()),
(7, 4, 'HPX360-I5-8-512', 1099.00, 18, 'available', NOW(), NOW()),
(8, 4, 'HPX360-I7-16-1TB', 1399.00, 10, 'available', NOW(), NOW()),
(9, 5, 'X1-I5-8-512', 1199.00, 14, 'available', NOW(), NOW()),
(10, 5, 'X1-I7-16-1TB', 1499.00, 8, 'available', NOW(), NOW()),
(11, 10, 'IP15P-128', 1199.00, 20, 'available', NOW(), NOW()),
(12, 10, 'IP15P-256', 1299.00, 15, 'available', NOW(), NOW()),
(13, 11, 'S24-128', 999.00, 25, 'available', NOW(), NOW()),
(14, 11, 'S24-256', 1099.00, 20, 'available', NOW(), NOW()),
(15, 12, 'PIX9-128', 899.00, 18, 'available', NOW(), NOW()),
(16, 12, 'PIX9-256', 999.00, 12, 'available', NOW(), NOW()),
(17, 20, 'IPADPRO-128', 1099.00, 20, 'available', NOW(), NOW()),
(18, 20, 'IPADPRO-256', 1299.00, 15, 'available', NOW(), NOW()),
(19, 21, 'TABS9-128', 899.00, 25, 'available', NOW(), NOW()),
(20, 21, 'TABS9-256', 999.00, 15, 'available', NOW(), NOW()),
(21, 30, 'DELL-U2720Q', 499.00, 12, 'available', NOW(), NOW()),
(22, 31, 'LG-UG32', 599.00, 10, 'available', NOW(), NOW()),
(23, 40, 'MX-KEYS-GRAY', 119.00, 30, 'available', NOW(), NOW()),
(24, 41, 'RZR-BW-V4', 149.00, 25, 'available', NOW(), NOW()),
(25, 50, 'MXM3S-GRAPHITE', 99.00, 35, 'available', NOW(), NOW()),
(26, 51, 'RZR-V2PRO', 129.00, 28, 'available', NOW(), NOW()),
(27, 60, 'SONY-XM5-BLK', 349.00, 18, 'available', NOW(), NOW()),
(28, 61, 'AIRPODS-PRO2', 249.00, 25, 'available', NOW(), NOW()),
(29, 70, 'ANKER-65W', 59.00, 40, 'available', NOW(), NOW()),
(30, 71, 'BASEUS-CABLE-15', 19.00, 100, 'available', NOW(), NOW()),
(31, 80, 'SPIGEN-RUGGED', 29.00, 50, 'available', NOW(), NOW()),
(32, 81, 'UAG-SLEEVE', 49.00, 35, 'available', NOW(), NOW())
ON CONFLICT (id) DO NOTHING;

-- ===========================
-- PRODUCT VARIANT ATTRIBUTES (composite key variantid + attributeid)
-- ===========================
INSERT INTO product_variant_attribute (variantid, attributeid, valueid) VALUES
(1, 1, 1),
(1, 12, 31),
(1, 13, 41),
(2, 1, 2),
(2, 12, 32),
(2, 13, 42),
(3, 1, 3),
(3, 12, 31),
(3, 13, 40),
(4, 1, 1),
(4, 12, 32),
(4, 13, 41),
(11, 1, 1),
(11, 24, 40),
(12, 1, 2),
(12, 24, 41),
(13, 1, 3),
(13, 24, 40),
(14, 1, 4),
(14, 24, 41),
(17, 1, 1),
(17, 33, 40),
(18, 1, 2),
(18, 33, 41),
(21, 50, 50),
(21, 52, 42),
(23, 70, 70),
(23, 73, 81),
(25, 80, 80),
(25, 81, 81),
(27, 90, 80),
(27, 91, 81),
(27, 92, 83),
(29, 100, 100),
(29, 101, 101),
(31, 110, 100),
(31, 111, 101),
(31, 112, 102)
ON CONFLICT (variantid, attributeid) DO NOTHING;

COMMIT;
