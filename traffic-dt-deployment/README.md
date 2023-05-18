# Traffic DT - Nasadenie

Adresár obsahuje jemne upravenú konfiguráciu vytvorenú skupinou AIL.
Originálna konfigurácia je dostupná na spoločnom github repozitári: https://github.com/TrafficDT/traffic-dt-deployment

Autori konfigurácie: Ivan Jatz, Jozef Juraško, Štefan Schindler

## Kroky na spustenie
1. Vytvoriť a upraviť .env konfiguráciu. Podkladom je súbor .env.example
2. Spustiť aplikáciu pomocou príkazu `docker compose up`

## Docker Images
### Head Collisions Detection 
[traffic-dt-head-collision-handler](https://hub.docker.com/r/ivanjatz/trafic-dt-head-collision-handler)

### Chain Collisions Detection 
[traffic-dt-chain-collision-detection](https://hub.docker.com/r/jozefjur/traffic-dt-chain-collision-detection)

### Integration Module 
[traffic-dt-integration-module](https://hub.docker.com/r/stefanschindler/traffic-dt-integration-module)

### Vehicle Simulator 
[traffic-dt-vehicle-simulator](https://hub.docker.com/r/stefanschindler/traffic-dt-vehicle-simulator)

### Visualization Processor 
[traffic-dt-visualization-processor](https://hub.docker.com/r/stefanschindler/traffic-dt-visualization-processor)

### Dashboard 
[traffic-dt-dashboard](https://hub.docker.com/r/stefanschindler/traffic-dt-dashboard)


