-- ============================================================
-- Virtual Plant Care Assistant — Full Database Setup Script
-- Database: VirtualPlantCareAssistantVSC1_1
-- ============================================================

-- 1. Create the Database
CREATE DATABASE IF NOT EXISTS VirtualPlantCareAssistantVSC1_1;
USE VirtualPlantCareAssistantVSC1_1;

-- 2. Create User Table
CREATE TABLE IF NOT EXISTS User (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(150) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL
);

-- 3. Create Plant Table
CREATE TABLE IF NOT EXISTS Plant (
    PlantId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    Name VARCHAR(100) NOT NULL,
    Species VARCHAR(100),
    WateringFrequencyDays INT DEFAULT 7,
    LastWatered DATETIME,
    FertilizationFrequencyDays INT DEFAULT 30,
    LastFertilized DATETIME,
    ImagePath VARCHAR(255),
    FOREIGN KEY (UserId) REFERENCES User(UserId) ON DELETE CASCADE
);

-- 4. Create CareLog Table
CREATE TABLE IF NOT EXISTS CareLog (
    LogId INT AUTO_INCREMENT PRIMARY KEY,
    PlantId INT NOT NULL,
    UserId INT NOT NULL,
    CareType VARCHAR(50) NOT NULL,   -- 'Watering' or 'Fertilization'
    CaredAt DATETIME NOT NULL,
    Notes VARCHAR(255),
    FOREIGN KEY (PlantId) REFERENCES Plant(PlantId) ON DELETE CASCADE
);

-- ============================================================
-- Setup Complete!
-- ============================================================
