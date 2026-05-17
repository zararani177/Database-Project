-- ============================================================
-- Virtual Plant Care Assistant — Sample Data Script
-- Use this to populate your database with some initial data
-- ============================================================

USE VirtualPlantCareAssistantVSC1_1;

-- 1. Insert a Sample User (Password is 'password123')
INSERT INTO User (Name, Email, Password) 
VALUES ('John Doe', 'john@example.com', 'password123')
ON DUPLICATE KEY UPDATE Name=VALUES(Name);

-- 2. Insert some Sample Plants for the first user
-- Note: This assumes John Doe got UserId 1.
INSERT INTO Plant (UserId, Name, Species, WateringFrequencyDays, LastWatered, FertilizationFrequencyDays, LastFertilized)
VALUES 
(1, 'Peace Lily', 'Spathiphyllum', 5, NOW(), 30, NOW()),
(1, 'Snake Plant', 'Dracaena trifasciata', 14, NOW(), 60, NOW()),
(1, 'Aloe Vera', 'Aloe barbadensis miller', 21, NOW(), 90, NOW());

-- 3. Insert some initial Care Logs
INSERT INTO CareLog (PlantId, UserId, CareType, CaredAt, Notes)
VALUES 
(1, 1, 'Watering', NOW(), 'Initial watering'),
(2, 1, 'Watering', NOW(), 'Initial watering'),
(3, 1, 'Watering', NOW(), 'Initial watering');

-- ============================================================
-- Sample Data Inserted!
-- ============================================================
