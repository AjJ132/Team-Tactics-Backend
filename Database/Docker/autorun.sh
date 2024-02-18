docker build -t teamtactics-mock-db .

docker run -d --name teamtactics-mock-db-container -p 5432:5432 teamtactics-mock-db