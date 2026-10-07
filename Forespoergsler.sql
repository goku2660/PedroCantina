-- Pedros Cantina – forespørgsler (opgave 5)
USE PedroCastina;

-- 1. Vis en månedsplan (her oktober 2026)
SELECT v.Dato, v.StartTid, v.SlutTid, m.Navn, m.ErLeder
FROM Månedsplan mp
JOIN Vagt v ON v.MånedsplanId = mp.Id
JOIN VagtMedarbejder vm ON vm.VagtId = v.Id
JOIN Medarbejder m ON m.Id = vm.MedarbejderId
WHERE mp.Måned = 10 AND mp.År = 2026
ORDER BY v.Dato, v.StartTid, m.Navn;

-- 2. Vis belastningen af de enkelte medarbejdere for en måned
SELECT m.Navn, COUNT(*) AS AntalVagter, COUNT(*) * 5 AS Timer
FROM Medarbejder m
JOIN VagtMedarbejder vm ON vm.MedarbejderId = m.Id
JOIN Vagt v ON v.Id = vm.VagtId
JOIN Månedsplan mp ON mp.Id = v.MånedsplanId
WHERE mp.Måned = 10 AND mp.År = 2026
GROUP BY m.Navn
ORDER BY AntalVagter DESC;

-- 3. Vis kontaktinfo på dem der ikke er på vagten, hvis en er syg (her vagt 1)
SELECT Navn, Telefon, ErLeder
FROM Medarbejder
WHERE Id NOT IN (SELECT MedarbejderId FROM VagtMedarbejder WHERE VagtId = 1)
ORDER BY Navn;

-- 4. Vis belastningen af medarbejdere hen over året
SELECT m.Navn, mp.Måned, COUNT(*) AS AntalVagter
FROM Medarbejder m
JOIN VagtMedarbejder vm ON vm.MedarbejderId = m.Id
JOIN Vagt v ON v.Id = vm.VagtId
JOIN Månedsplan mp ON mp.Id = v.MånedsplanId
WHERE mp.År = 2026
GROUP BY m.Navn, mp.Måned
ORDER BY m.Navn, mp.Måned;

-- 5a. Lav en ny månedsplan
INSERT INTO Månedsplan (Måned, År) VALUES (11, 2026);

-- 5b. Lav en vagt i planen (bruger Id på den nye plan)
INSERT INTO Vagt (MånedsplanId, Dato, StartTid, SlutTid)
VALUES ((SELECT Id FROM Månedsplan WHERE Måned = 11 AND År = 2026), '2026-11-01', '09:00', '14:00');

-- 5c. Sæt medarbejdere på vagten (Pedro og Anna)
INSERT INTO VagtMedarbejder (VagtId, MedarbejderId)
VALUES ((SELECT Id FROM Vagt WHERE Dato = '2026-11-01' AND StartTid = '09:00'), 1),
       ((SELECT Id FROM Vagt WHERE Dato = '2026-11-01' AND StartTid = '09:00'), 3);

-- 5d. Juster planen: Anna (3) er syg, Bo (4) tager vagten
UPDATE VagtMedarbejder
SET MedarbejderId = 4
WHERE VagtId = (SELECT Id FROM Vagt WHERE Dato = '2026-11-01' AND StartTid = '09:00')
  AND MedarbejderId = 3;
