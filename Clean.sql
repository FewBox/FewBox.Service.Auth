SELECT 
CONCAT('TRUNCATE TABLE ',TABLE_NAME,';') AS truncateCommand
FROM information_schema.TABLES 
WHERE TABLE_SCHEMA = 'auth';