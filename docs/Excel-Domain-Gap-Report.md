# Excel-Domain Gap Report

## Data Extraction Snapshot

Sheets were parsed with ClosedXML. For each sheet: row-1 headers + sample rows 2-5 (if present) were extracted.

### Data collection from 1-1-2023 till 31-12-2023.xlsx
- **Sheet:** Site Assets Data Count
  - Headers: ShortCode \| Site OMC Name \| Site Code \| BSC Name \| Subcontractor \| Maintenance Area \| Region \| Subregion \| Subcontractor Office \| Announcement Date \| On / Off  Air \| Site Coordinates X, Y \| Address Detailes \| Sharing \| Tower \| Radio Equipment Data \| DC Power \| AC Power \| Cooling System \| Fire Panel \| ZTE Remote Monitoring System \| General Data \| Status
  - Row 2: Radio Equipment Data=Transmission; DC Power=Battery Data
  - Row 3: Sharing=Sharing; Tower=Structure Type ( GF Tower/ RT Tower/GF Monopole/ RT Monopole/Palm Tree /Quick Site/ Mast/Billboard/Flage Pole/MS/Special Camouflage); Radio Equipment Data=Site Transmission Type; DC Power=Batteries Type Brand; AC Power=Site Power Configuration; Cooling System=A/C1 Type /HP; Fire Panel=Type ; ZTE Remote Monitoring System=Camera Functionality Status; General Data=Telecom Egypt / police station / Radio TV / Factory Name / Company Name
  - Row 4: ShortCode=3564DE; Site OMC Name=DK-MIT-MAHMUD; Site Code=C1_3564DE_PH13; BSC Name=MANSOURA01; Subcontractor=MOBISERVE; Maintenance Area=AGLI; Region=Delta ; Subregion=Delta North; Subcontractor Office=Mansoura; Announcement Date=40695; On / Off  Air=On Air; Site Coordinates X, Y=31.58812,31.026328; Address Detailes=Tanah Road - mit mahmoud village; Sharing=Not Shared; Tower=(GF) Tower; Radio Equipment Data=MW; DC Power=SBS; AC Power=Orange EC; Cooling System=Grill; Fire Panel=ZETA; ZTE Remote Monitoring System=Not Exist; General Data=NA
  - Row 5: ShortCode=4390DE; Site OMC Name=DK-MNS-HOMYAT; Site Code=C2_4390DE_PH18; BSC Name=MANSOURA01; Subcontractor=MOBISERVE; Maintenance Area=AGLI; Region=Delta ; Subregion=Delta North; Subcontractor Office=Mansoura; Announcement Date=43639; On / Off  Air=On Air; Site Coordinates X, Y=31.184152,31.110512; Address Detailes=In Mansoura City , Beside El-Homiat Hospital & Car Ghaztac Station , Dkahlia Governorate; Sharing=ORG In ET; Tower=(RT) Stub Tower; Radio Equipment Data=MW; DC Power=SBS; AC Power=ET EC; Cooling System=Huawei / 0.5 HP; Fire Panel=Outdoor zone; ZTE Remote Monitoring System=Not Exist; General Data=NA
- **Sheet:** Power Data
  - Headers: # \| Site Code \| BSC Code \| BSC Name \| OEG Site Name \| TE Name \| No . Of locations \| Site Type \| A/C Units \| A/C Capacity \| Rectifier No. \| Routers \| GPS \| ADM \| Modem \| TX Indoor Qty. \| TX Indoor Type \| BSC Qty \| BSC Type \| BTS Vendor \| BTS QTY \| 2G RF  Modules \| 3G RF Modules \| 4G RF Modules \| Cabinet Type (Y/N) Y=Rectifier in Cabinet \| Battery Type \| Battery Strings Qty \| Rectifier Type \| No of Modules \| TE Current C.B Rate \| Date Of Visit \| Contact Person
  - Row 2: #=1; Site Code=3564DE; BSC Code=8004DE; BSC Name=MANSOURA01; OEG Site Name=DK-MIT-MAHMUD; TE Name=NA; No . Of locations=1; Site Type=Green Field; A/C Units=Grill; A/C Capacity=0/Grill; Rectifier No.=1; Routers=0; GPS=Not Exist; ADM=Not Exist; Modem=0; TX Indoor Qty.=1; TX Indoor Type=NEC IPaso Link VR4; BSC Qty=NA; BSC Type=NA; BTS Vendor=NSN; BTS QTY=1; 2G RF  Modules=3; 3G RF Modules=2; 4G RF Modules=1; Cabinet Type (Y/N) Y=Rectifier in Cabinet=N; Battery Type=SBS; Battery Strings Qty=1; Rectifier Type=Huawei; No of Modules=4; TE Current C.B Rate=NA; Date Of Visit=44859
  - Row 3: #=2; Site Code=4390DE; BSC Code=8004DE; BSC Name=MANSOURA01; OEG Site Name=DK-MNS-HOMYAT; TE Name=NA; No . Of locations=1; Site Type=Roof Top; A/C Units=Huawei / 0.5 HP; A/C Capacity=1/6 HP; Rectifier No.=1; Routers=0; GPS=Not Exist; ADM=Not Exist; Modem=0; TX Indoor Qty.=1; TX Indoor Type=NEC IPaso Link VR4; BSC Qty=NA; BSC Type=NA; BTS Vendor=NSN; BTS QTY=1; 2G RF  Modules=3; 3G RF Modules=1; 4G RF Modules=1; Cabinet Type (Y/N) Y=Rectifier in Cabinet=Y / Huawei Cabinet; Battery Type=SBS; Battery Strings Qty=1; Rectifier Type=Huawei; No of Modules=3; TE Current C.B Rate=NA; Date Of Visit=26/10/2022
  - Row 4: #=3; Site Code=1009DE; BSC Code=8004DE; BSC Name=MANSOURA01; OEG Site Name=DK-I-MNSRASHP2; TE Name=NA; No . Of locations=1; Site Type=Indoor; A/C Units=Huawei / 1/6 HP; A/C Capacity=1/6HP; Rectifier No.=1; Routers=0; GPS=Not Exist; ADM=Not Exist; Modem=0; TX Indoor Qty.=1; TX Indoor Type=NEC I Paso Link 200; BSC Qty=NA; BSC Type=NA; BTS Vendor=NSN; BTS QTY=3; 2G RF  Modules=1; 3G RF Modules=1; 4G RF Modules=1; Cabinet Type (Y/N) Y=Rectifier in Cabinet=Y / Huawei Cabinet; Battery Type=SBS; Battery Strings Qty=1; Rectifier Type=Huawei; No of Modules=2; TE Current C.B Rate=NA; Date Of Visit=44861
  - Row 5: #=4; Site Code=4175DE; BSC Code=7007DE; BSC Name=MAHALLA01; OEG Site Name=GH-MAHLA-7BNAT ; TE Name=NA; No . Of locations=1; Site Type=Roof Top; A/C Units=Huawei / 0.5 HP; A/C Capacity=0.25 HP; Rectifier No.=1; Routers=0; GPS=Not Exist; ADM=Not Exist; Modem=0; TX Indoor Qty.=1; TX Indoor Type=NEC I Paso Link VR2; BSC Qty=NA; BSC Type=NA; BTS Vendor=NSN; BTS QTY=1; 2G RF  Modules=5; 3G RF Modules=3; 4G RF Modules=2; Cabinet Type (Y/N) Y=Rectifier in Cabinet=Y / Huawei Cabinet; Battery Type=Batteries not installed; Battery Strings Qty=Batteries not installed; Rectifier Type=Huawei; No of Modules=4; TE Current C.B Rate=NA; Date Of Visit=44858
- **Sheet:** Site Radio Data
  - Headers: Short Code \| Name \| Date Of Visit \| Sector Technology \| Sector number \| Site Topology \| Building Height (m) \| Support Height (m) \| Support ID \| Azimuth \| Antenna Type \| Antenna Sharing \| RRU/RF/RRH Solution \| Combiner/ Diplexer ID \| Mast Head Amplifier \| Bias-T \| HBA(m) \| Bracket Tilt \| Elect Tilt \| Feeder Size / Cable Type \| Feeder Length \| Remarks
  - Row 2: Short Code=3564DE; Name=DK-MIT-MAHMUD; Date Of Visit=44859; Sector Technology=GU900; Sector number=1; Site Topology=Green Field - Tower; Building Height (m)=0; Support Height (m)=45; Support ID=1; Azimuth=30; Antenna Type=JW588 (6-Port); Antenna Sharing=No; RRU/RF/RRH Solution=Ground Radio Unit; Combiner/ Diplexer ID=Not Exist; Mast Head Amplifier=Not Exist; Bias-T=Not Exist; HBA(m)=40; Bracket Tilt=0; Elect Tilt=5; Feeder Size / Cable Type=7/8 inch; Feeder Length=55
  - Row 3: Short Code=3564DE; Name=DK-MIT-MAHMUD; Date Of Visit=44859; Sector Technology=GU900; Sector number=2; Site Topology=Green Field - Tower; Building Height (m)=0; Support Height (m)=45; Support ID=1; Azimuth=130; Antenna Type=JW588 (6-Port); Antenna Sharing=No; RRU/RF/RRH Solution=Ground Radio Unit; Combiner/ Diplexer ID=Not Exist; Mast Head Amplifier=Not Exist; Bias-T=Not Exist; HBA(m)=40; Bracket Tilt=0; Elect Tilt=5; Feeder Size / Cable Type=1 5/8 inch; Feeder Length=55
  - Row 4: Short Code=3564DE; Name=DK-MIT-MAHMUD; Date Of Visit=44859; Sector Technology=GU900; Sector number=3; Site Topology=Green Field - Tower; Building Height (m)=0; Support Height (m)=45; Support ID=1; Azimuth=240; Antenna Type=Huawei (6-Port); Antenna Sharing=No; RRU/RF/RRH Solution=Ground Radio Unit; Combiner/ Diplexer ID=Not Exist; Mast Head Amplifier=Not Exist; Bias-T=Not Exist; HBA(m)=40; Bracket Tilt=0; Elect Tilt=0; Feeder Size / Cable Type=1 5/8 inch; Feeder Length=55
  - Row 5: Short Code=3564DE; Name=DK-MIT-MAHMUD; Date Of Visit=44859; Sector Technology=D1800; Sector number=1; Site Topology=Green Field - Tower; Building Height (m)=0; Support Height (m)=45; Support ID=1; Azimuth=30; Antenna Type=JW588 (6-Port); Antenna Sharing=No; RRU/RF/RRH Solution=Ground Radio Unit; Combiner/ Diplexer ID=Not Exist; Mast Head Amplifier=Not Exist; Bias-T=Not Exist; HBA(m)=40; Bracket Tilt=0; Elect Tilt=1; Feeder Size / Cable Type=1 1/4 inch; Feeder Length=55
- **Sheet:** Site TX Data
  - Headers: Short Code \| Local Site Name \| Directions Site Name \| Directions Site Code \| Mast No. \| TX HBA \| TX Azimuth \| Antenna Diameter [m] \| Side Arm Height [m] \| Side Arm Diameter \| IP Address \| Link Type \| Link Model \| Band \| Configuration \| Modulation \| Capacity [Mb/s] \| Tx Frequency [KHz] \| Rx Frequency [KHz] \| Tx Power [dBm] \| Rx Power [dBm] \| ODU Model \| ODU S/N \| Opposite site ODU S/N \| Antenna Reference \| Polarization \| Remarks
  - Row 2: Short Code=3564DE; Local Site Name=DK-MIT-MAHMUD; Directions Site Name=DKIRNIS-STH; Directions Site Code=0988DE; Mast No.=1; TX HBA=42; TX Azimuth=310; Antenna Diameter [m]=1.2; Side Arm Height [m]=1; Side Arm Diameter=4"; IP Address=10.45.198.203; Link Type=PDH; Link Model=NEC IPaso Link VR4; Band=26; Configuration=1+0; Modulation=256QAM; Capacity [Mb/s]=14; Tx Frequency [KHz]=25858; Rx Frequency [KHz]=24850; Tx Power [dBm]=15; Rx Power [dBm]=-42.8; ODU Model=NWA-078621-AB0; ODU S/N=19999; Opposite site ODU S/N=19883; Antenna Reference=TRP-25G-1D; Polarization=V
  - Row 3: Short Code=3564DE; Local Site Name=DK-MIT-MAHMUD; Directions Site Name=DK-SALAMUN; Directions Site Code=0599DE; Mast No.=1; TX HBA=43; TX Azimuth=10; Antenna Diameter [m]=0.6; Side Arm Height [m]=1; Side Arm Diameter=2"; IP Address=10.45.198.114; Link Type=PDH; Link Model=NEC IPaso Link VR4; Band=23; Configuration=1+0; Modulation=32QAM; Capacity [Mb/s]=28; Tx Frequency [KHz]=22162; Rx Frequency [KHz]=23170; Tx Power [dBm]=16; Rx Power [dBm]=-46.3; ODU Model=NWA-058387-AA0; ODU S/N=81986; Opposite site ODU S/N=76756; Antenna Reference=TRP-23G-1D; Polarization=V
  - Row 4: Short Code=4390DE; Local Site Name=DK-MNS-HOMYAT; Directions Site Name= DK-MANSOURAH5 (V); Directions Site Code=0062DE; Mast No.=1; TX HBA=40; TX Azimuth=180; Antenna Diameter [m]=0.6; Side Arm Height [m]=2; Side Arm Diameter=4"; IP Address=10.45.198.209; Link Type=PDH; Link Model=NEC IPaso Link VR4; Band=38; Configuration=1 + 0 XPIC; Modulation=QPSK; Capacity [Mb/s]=14; Tx Frequency [KHz]=37177; Rx Frequency [KHz]=38437; Tx Power [dBm]=13.8; Rx Power [dBm]=-40; ODU Model=NWA-078624-AB0; ODU S/N=39878; Opposite site ODU S/N=64351; Antenna Reference=TRP-38G-1D; Polarization=V
  - Row 5: Short Code=4390DE; Local Site Name=DK-MNS-HOMYAT; Directions Site Name= DK-MANSOURAH5 (H); Directions Site Code=0062DE; Mast No.=1; TX HBA=40; TX Azimuth=180; Antenna Diameter [m]=0.6; Side Arm Height [m]=2; Side Arm Diameter=4"; IP Address=10.45.198.209; Link Type=PDH; Link Model=NEC IPaso Link VR4; Band=38; Configuration=1 + 0 XPIC; Modulation=QPSK; Capacity [Mb/s]=14; Tx Frequency [KHz]=37177; Rx Frequency [KHz]=38437; Tx Power [dBm]=14; Rx Power [dBm]=-39.5; ODU Model=NWA-078624-AB0; ODU S/N=61452; Opposite site ODU S/N=65948; Antenna Reference=TRP-38G-1D; Polarization=H
- **Sheet:** Site Sharing Data
  - Headers: Short Code \| Name \| Site Host \| Host Code \| Site Guests \| Topology \| TX Enclosure \| Sharing Count Radio Antenna \| Radio Azimuth 1 \| Radio HBA 1 \| Radio Azimuth 2 \| Radio HBA 2 \| Radio Azimuth 3 \| Radio HBA 3 \| Radio Azimuth 4 \| Radio HBA 4 \| Radio Azimuth 5 \| Radio HBA 5 \| Radio Azimuth 6 \| Radio HBA 6 \| Radio Azimuth 7 \| Radio HBA 7 \| Radio Azimuth 8 \| Radio HBA 8 \| Sharing Count Tx Antenna \| TX Azimuth 1 \| TX HBA 1 \| TX Azimuth 2 \| TX HBA 2 \| TX Azimuth 3 \| TX HBA 3 \| TX Azimuth 4 \| TX HBA 4 \| TX Azimuth 5 \| TX HBA 5 \| TX Azimuth 6 \| TX HBA 6 \| TX Azimuth 7 \| TX HBA 7 \| TX Azimuth 8 \| TX HBA 8 \| TX Azimuth 9 \| TX HBA 9 \| Remarks
  - Row 2: Short Code=0658DE; Name=BH-MNSHYAT-KHYAT; Site Host=orange; Host Code=0658DE; Site Guests=Vodafone; Topology=Green Field - Tower; TX Enclosure=19" Rack (Shelter); Sharing Count Radio Antenna=4; Radio Azimuth 1=80; Radio HBA 1=42; Radio Azimuth 2=200; Radio HBA 2=42; Radio Azimuth 3=250; Radio HBA 3=42; Radio Azimuth 4=355; Radio HBA 4=42; Radio Azimuth 5=NA; Radio HBA 5=NA; Radio Azimuth 6=NA; Radio HBA 6=NA; Radio Azimuth 7=NA; Radio HBA 7=NA; Radio Azimuth 8=NA; Radio HBA 8=NA; Sharing Count Tx Antenna=1; TX Azimuth 1=190; TX HBA 1=35; TX Azimuth 2=NA; TX HBA 2=NA; TX Azimuth 3=NA; TX HBA 3=NA; TX Azimuth 4=NA; TX HBA 4=NA; TX Azimuth 5=NA; TX HBA 5=NA; TX Azimuth 6=NA; TX HBA 6=NA; TX Azimuth 7=NA; TX HBA 7=NA; TX Azimuth 8=NA; TX HBA 8=NA; TX Azimuth 9=NA; TX HBA 9=NA
  - Row 3: Short Code=3444DE; Name=MIT-YAEESH; Site Host=Orange; Host Code=3444DE; Site Guests=ET; Topology=Green Field - Tower; TX Enclosure=OD Cabinet; Sharing Count Radio Antenna=3; Radio Azimuth 1=30; Radio HBA 1=60; Radio Azimuth 2=180; Radio HBA 2=60; Radio Azimuth 3=280; Radio HBA 3=60; Radio Azimuth 4=NA; Radio HBA 4=NA; Radio Azimuth 5=NA; Radio HBA 5=NA; Radio Azimuth 6=NA; Radio HBA 6=NA; Radio Azimuth 7=NA; Radio HBA 7=NA; Radio Azimuth 8=NA; Radio HBA 8=NA; Sharing Count Tx Antenna=NA; TX Azimuth 1=NA; TX HBA 1=NA; TX Azimuth 2=NA; TX HBA 2=NA; TX Azimuth 3=NA; TX HBA 3=NA; TX Azimuth 4=NA; TX HBA 4=NA; TX Azimuth 5=NA; TX HBA 5=NA; TX Azimuth 6=NA; TX HBA 6=NA; TX Azimuth 7=NA; TX HBA 7=NA; TX Azimuth 8=NA; TX HBA 8=NA; TX Azimuth 9=NA; TX HBA 9=NA
  - Row 4: Short Code=3047DE; Name=QL-MASHTOL-2; Site Host=Orange; Host Code=3047DE; Site Guests=Vodafone; Topology=Roof Top - Mast; TX Enclosure=Shelter rack; Sharing Count Radio Antenna=2; Radio Azimuth 1=60; Radio HBA 1=23; Radio Azimuth 2=250; Radio HBA 2=23; Sharing Count Tx Antenna=1; TX Azimuth 1=290; TX HBA 1=23
  - Row 5: Short Code=0913DE; Name=SK-BELBES-WST; Site Host=ORANGE; Host Code=0913DE; Site Guests=WE; Topology=Roof Top _ Stub tower; TX Enclosure=19" Rack (Shelter); Sharing Count Radio Antenna=3; Radio Azimuth 1=60; Radio HBA 1=27; Radio Azimuth 2=195; Radio HBA 2=25; Radio Azimuth 3=315; Radio HBA 3=25; Radio Azimuth 4=NA; Radio HBA 4=NA; Radio Azimuth 5=NA; Radio HBA 5=NA; Radio Azimuth 6=NA; Radio HBA 6=NA; Radio Azimuth 7=NA; Radio HBA 7=NA; Radio Azimuth 8=NA; Radio HBA 8=NA; Sharing Count Tx Antenna=NA; TX Azimuth 1=NA; TX HBA 1=NA; TX Azimuth 2=NA; TX HBA 2=NA; TX Azimuth 3=NA; TX HBA 3=NA; TX Azimuth 4=NA; TX HBA 4=NA; TX Azimuth 5=NA; TX HBA 5=NA; TX Azimuth 6=NA; TX HBA 6=NA; TX Azimuth 7=NA; TX HBA 7=NA; TX Azimuth 8=NA; TX HBA 8=NA; TX Azimuth 9=NA; TX HBA 9=NA

### Delta Sites (1).xlsx
- **Sheet:** Sheet2
  - Headers: Short Code \| Site Name \| Office \| Nodal Degree \| Rectifier Brand \| Battery Type/Volt/AH \| No of String \| Batteries Status \| Announcement Date
  - Row 2: Short Code=1067DE; Site Name=GH-I-ORUBA-MLL; Office=TANTA; Nodal Degree=1+0; Rectifier Brand=Delta 4 - HUAWEI; Battery Type/Volt/AH=SBS 12 V / 92,40 AH; No of String=1,1; Batteries Status=Faulty Batteries; Announcement Date=40696
  - Row 3: Short Code=0058DE; Site Name=DK-MANSURAH1; Office=MANSOURA; Nodal Degree=1+0; Rectifier Brand=Delta 4; Battery Type/Volt/AH=SBS 12 V / 170 AH; No of String=3; Batteries Status=Faulty Batteries; Announcement Date=40757
  - Row 4: Short Code=4390DE; Site Name=DK-MNS-HOMYAT; Office=MANSOURA; Nodal Degree=1+0; Rectifier Brand=HUAWEI (TP48300A); Battery Type/Volt/AH=SBS 12 V / 170 AH; No of String=1; Batteries Status=Faulty Batteries; Announcement Date=40695
  - Row 5: Short Code=0907DE; Site Name=GH-ZIFTA-NTH; Office=SHIBIN; Nodal Degree=1+0; Rectifier Brand=NSN; Battery Type/Volt/AH=SBS 12 V / 170 AH; No of String=2,2; Batteries Status=Faulty Batteries; Announcement Date=41462
- **Sheet:** Sheet1
  - Headers: Short Code \| Site Name \| SC Office \| OZ \| Nodal Deg.
  - Row 2: Short Code=0700DE; Site Name=QL-TANAN; SC Office=BENHA; OZ=Delta South; Nodal Deg.=1
  - Row 3: Short Code=0700DE; Site Name=TANAN Tx; SC Office=BENHA; OZ=Delta South; Nodal Deg.=19
  - Row 4: Short Code=0676DE; Site Name=ZAWYET-RAZEN Tx; SC Office=SHIBIN; OZ=Delta South; Nodal Deg.=29
  - Row 5: Short Code=0676DE; Site Name=MF-ZAWYET-RAZEN; SC Office=SHIBIN; OZ=Delta South; Nodal Deg.=1

### GH-BDT_BDT.xlsx
- **Sheet:** BDT sheet
  - Headers: (none in row 1)
  - Row 2: col1=Battery Discharge Test (Autonomy Control)
  - Row 4: col1=Subcontractor:; col7=Sub-office: ; col9=Tanta; col13=Date: 
  - Row 5: col1=Site Name: ; col7=Site Code:; col13=Time in:; col15=0.4583333333333333
- **Sheet:** Power Alarm
  - Headers: (none in row 1)
  - Row 2: col5=Eng. Name:
  - Row 3: col5=CAP code
  - Row 4: col5=Comment
- **Sheet:** Config
  - Headers: (none in row 1)
  - Row 2: col3=For NSN Sites; col7=For HUW Sites
  - Row 4: col3=Site name / Code; col4=#NAME?; col7=Site name / Code; col10=Example
  - Row 5: col3=Rectifier type; col4=Delta 2; col7=Rectifier type; col8=Delta 2
- **Sheet:** Summary 
  - Headers: Week \| Ser \| Site Name \| Short Code \| On Air Date \| Nodal Degree \| PLVD Value (LLVD For Huawei) adjusted after finishing the test \| Linked sites name codes \| Type \| Site Category (Shelter/OD/Grill) \| Power Source \| # of BSC \| BSC Type \| # of BTS \| Type2 \| # of GSM/MRFU/RF \| # of DSC/MRFU/RF \| # of MW \| MW Type \| # of SDH \| # of ADM \| # of Routers \| AC1 Type \| AC HP \| AC2 Type \| AC HP3 \| 3G Type \| No. Of 3G RF \| 4G Type \| No. Of 4G RF \| Orange office \| Subcontractor \| Office \| Area \| Network \| Rectifier Brand \| # of Modules \| Battery Brand \| Battery Volt \| Battery Ampere Hour \| No of String \| No of Batteries \| Start Volt \| Start Amp \| Batteries Charnging current limit \| End Volt \| End Amp \| Discharge time( Mins) \| Reason for Test stop \| Test Date \| Reason for Repeated BDT \| Cap request # \| Comment
  - Row 2: Nodal Degree=NA; PLVD Value (LLVD For Huawei) adjusted after finishing the test=NA; Type=BRONZE; Power Source=EC; # of BSC=0; BSC Type=NA; # of BTS=1; Type2=NSN; # of GSM/MRFU/RF=2; # of DSC/MRFU/RF=0; # of MW=1; MW Type=iPASOLINK vr4; # of SDH=0; # of ADM=0; # of Routers=0; AC1 Type=Not Exist; AC HP=Not Exist; AC2 Type=Not Exist; AC HP3=Not Exist; 3G Type=NSN; No. Of 3G RF=2; 4G Type=NA; No. Of 4G RF=1; Orange office=AGLI; Subcontractor=MBV; Office=TANTA; Area=Delta North; Network=NSN; Rectifier Brand=DELTA2; Batteries Charnging current limit=180; Reason for Test stop=BATTERIES ARE DISCHARGED; Reason for Repeated BDT=Cycle

### GH-DE  Checklist.xlsx
- **Sheet:** site's reading
  - Headers: (none in row 1)
  - Row 2: col3=`
  - Row 5: col1=Site Name: ; col9=Site Code: ; col17=Date: 
- **Sheet:** Common checklist
  - Headers: (none in row 1)
  - Row 2: col2=Important Tips
  - Row 3: col3= All Metal panels must be IP 65 or 66 Schneider or ETA or IDA or INKOL , Otherwise "Orange approval must be taken before using"
  - Row 4: col3=All CBs must be Schneider or Merlin Gerin, Otherwise  "Orange approval must be taken before using"
- **Sheet:** Panorama
  - Headers: Shelter panorama
  - Sample rows 2-5: (no non-empty sample cells found)
- **Sheet:** Sheet1
  - Headers: (none in row 1)
  - Sample rows 2-5: (no non-empty sample cells found)
- **Sheet:** Sheet2
  - Headers: (none in row 1)
  - Sample rows 2-5: (no non-empty sample cells found)
- **Sheet:** Tower Panorama
  - Headers: Tower panorama
  - Sample rows 2-5: (no non-empty sample cells found)
- **Sheet:** Before & after
  - Headers: (none in row 1)
  - Row 2: col2=Before; col10=After
- **Sheet:** Pending Res.
  - Headers: Pending Reserves
  - Row 2: Pending Reserves=No.
  - Row 3: Pending Reserves=1
  - Row 4: Pending Reserves=2
  - Row 5: Pending Reserves=3
- **Sheet:** unused assets
  - Headers: unused assets
  - Row 2: unused assets=No.
  - Row 3: unused assets=1
  - Row 4: unused assets=2
  - Row 5: unused assets=3
- **Sheet:** alarms capture
  - Headers: BTS capture ( Alarms ) \| 3G capture \| MW capture
  - Sample rows 2-5: (no non-empty sample cells found)
- **Sheet:** Audit matrix SQI
  - Headers: File \| Network Audit Checklist (applicable on entire radio sites)
  - Row 2: File=File Version; Network Audit Checklist (applicable on entire radio sites)=V10R00
  - Row 3: File=Department; Network Audit Checklist (applicable on entire radio sites)=Technology / Access Network Support / Network Audit
  - Row 4: File=Creator Name; Network Audit Checklist (applicable on entire radio sites)=Samer Zaki
  - Row 5: File=Creator Title; Network Audit Checklist (applicable on entire radio sites)=Network Audit Manager

### GH-DE Data Collection.xlsx
- **Sheet:** Site Sharing Data
  - Headers: Short Code \| Name \| Site Host \| Host Code \| Site Guests \| Topology \| TX Enclosure \| Sharing Count Radio Antenna \| Radio Azimuth 1 \| Radio HBA 1 \| Radio Azimuth 2 \| Radio HBA 2 \| Radio Azimuth 3 \| Radio HBA 3 \| Radio Azimuth 4 \| Radio HBA 4 \| Radio Azimuth 5 \| Radio HBA 5 \| Radio Azimuth 6 \| Radio HBA 6 \| Radio Azimuth 7 \| Radio HBA 7 \| Radio Azimuth 8 \| Radio HBA 8 \| Sharing Count Tx Antenna \| TX Azimuth 1 \| TX HBA 1 \| TX Azimuth 2 \| TX HBA 2 \| TX Azimuth 3 \| TX HBA 3 \| TX Azimuth 4 \| TX HBA 4 \| TX Azimuth 5 \| TX HBA 5 \| TX Azimuth 6 \| TX HBA 6 \| TX Azimuth 7 \| TX HBA 7 \| TX Azimuth 8 \| TX HBA 8 \| TX Azimuth 9 \| TX HBA 9 \| Remarks
  - Sample rows 2-5: (no non-empty sample cells found)
- **Sheet:** Site TX Data
  - Headers: Short Code \| Local Site Name \| Directions Site Name \| Directions Site Code \| Mast No. \| TX HBA \| TX Azimuth \| Antenna Diameter [m] \| Side Arm Height [m] \| Side Arm Diameter \| IP Address \| Link Type \| Link Model \| Band \| Configuration \| Modulation \| Capacity [Mb/s] \| Tx Frequency [KHz] \| Rx Frequency [KHz] \| Tx Power [dBm] \| Rx Power [dBm] \| ODU Model \| ODU S/N \| Opposite site ODU S/N \| Antenna Reference \| Polarization \| Remarks
  - Sample rows 2-5: (no non-empty sample cells found)
- **Sheet:** Site Radio Data
  - Headers: Short Code \| Name \| Date Of Visit \| Sector Technology \| Sector number \| Site Topology \| Building Height (m) \| Support Height (m) \| Support ID \| Azimuth \| Antenna Type \| Antenna Sharing \| RRU/RF/RRH Solution \| Combiner/ Diplexer ID \| Mast Head Amplifier \| Bias-T \| HBA(m) \| Bracket Tilt \| Elect Tilt \| Feeder Size / Cable Type \| Feeder Length \| Remarks
  - Sample rows 2-5: (no non-empty sample cells found)
- **Sheet:** Power Data
  - Headers: # \| Site Code \| BSC Code \| BSC Name \| OEG Site Name \| TE Name \| No . Of locations \| Site Type \| A/C Units \| A/C Capacity \| Rectifier No. \| Routers \| GPS \| ADM \| Modem \| TX Indoor Qty. \| TX Indoor Type \| BSC Qty \| BSC Type \| BTS Vendor \| BTS QTY \| 2G RF  Modules \| 3G RF Modules \| 4G RF Modules \| Cabinet Type (Y/N) Y=Rectifier in Cabinet \| Battery Type \| Battery Strings Qty \| Rectifier Type \| No of Modules \| TE Current C.B Rate \| Date Of Visit \| Contact Person
  - Sample rows 2-5: (no non-empty sample cells found)
- **Sheet:** Site Assets Data Count
  - Headers: ShortCode \| Site OMC Name \| Site Code \| BSC Name \| Subcontractor \| Maintenance Area \| Region \| Subregion \| Subcontractor Office \| Announcement Date \| On / Off  Air \| Site Coordinates X, Y \| Address Detailes \| Sharing \| Tower \| Radio Equipment Data \| DC Power \| AC Power \| Cooling System \| Fire Panel \| ZTE Remote Monitoring System \| General Data \| Status
  - Row 2: Radio Equipment Data=Transmission; DC Power=Battery Data
  - Row 3: Sharing=Sharing; Tower=Structure Type ( GF Tower/ RT Tower/GF Monopole/ RT Monopole/Palm Tree /Quick Site/ Mast/Billboard/Flage Pole/MS/Special Camouflage); Radio Equipment Data=Site Transmission Type; DC Power=Batteries Type Brand; AC Power=Site Power Configuration; Cooling System=A/C1 Type /HP; Fire Panel=Type ; ZTE Remote Monitoring System=Camera Functionality Status; General Data=Telecom Egypt / police station / Radio TV / Factory Name / Company Name
- **Sheet:** RF Status
  - Headers: Site code \| site name \| total RF In site \| RF on tower Number \| RF on ground Number \| Number RF sector carry \| band for RF on tower \| band for RF on ground \| comment
  - Sample rows 2-5: (no non-empty sample cells found)

### Power Data.xlsx
- **Sheet:** Power Data
  - Headers: # \| Site Code \| BSC Code \| BSC Name \| OEG Site Name \| TE Name \| No . Of locations \| Site Type \| A/C Units \| A/C Capacity \| Rectifier No. \| Routers \| GPS \| ADM \| Modem \| TX Indoor Qty. \| TX Indoor Type \| BSC Qty \| BSC Type \| BTS Vendor \| BTS QTY \| 2G RF  Modules \| 3G RF Modules \| 4G RF Modules \| Cabinet Type (Y/N) Y=Rectifier in Cabinet \| Battery Type \| Battery Strings Qty \| Rectifier Type \| No of Modules \| TE Current C.B Rate \| Date Of Visit \| Contact Person
  - Row 2: #=1002; Site Code=0935DE; BSC Code=8018DE; BSC Name=BENHA01; OEG Site Name=SK-ARB-JUHAINA; TE Name=NA; No . Of locations=1; Site Type= BTS; A/C Units=0/Grill; A/C Capacity=0/Grill; Rectifier No.=1; Routers=Not Exist; GPS=Not Exist; ADM=Not Exist; Modem=Not Exist; TX Indoor Qty.=1; TX Indoor Type=NEC I Paso Link 200; BSC Qty=0; BSC Type=Not Exist; BTS Vendor=NSN; BTS QTY=1; 2G RF  Modules=3; 3G RF Modules=4; 4G RF Modules=2; Cabinet Type (Y/N) Y=Rectifier in Cabinet=N; Battery Type=SBS; Battery Strings Qty=2; Rectifier Type=Delta 2; No of Modules=4; TE Current C.B Rate=NA; Date Of Visit=44563
  - Row 3: #=1003; Site Code=3730DE; BSC Code=8018DE; BSC Name=BENHA01; OEG Site Name=QL-SHBRA-SHIHB; TE Name=NA; No . Of locations=1; Site Type= BTS; A/C Units=2; A/C Capacity=3HP , 3HP; Rectifier No.=1; Routers=Not Exist; GPS=Not Exist; ADM=Not Exist; Modem=Not Exist; TX Indoor Qty.=1; TX Indoor Type=NEC I Paso Link 200; BSC Qty=0; BSC Type=Not Exist; BTS Vendor=NSN; BTS QTY=1; 2G RF  Modules=2; 3G RF Modules=4; 4G RF Modules=1; Cabinet Type (Y/N) Y=Rectifier in Cabinet=N; Battery Type=Marathon; Battery Strings Qty=3; Rectifier Type=Delta 3; No of Modules=4; TE Current C.B Rate=NA; Date Of Visit=44565
  - Row 4: #=1004; Site Code=0001DE; BSC Code=8018DE; BSC Name=BENHA01; OEG Site Name=QL-QALIUB; TE Name=QALIUB EXCHANGE; No . Of locations=1; Site Type= BTS; A/C Units=0/Grill; A/C Capacity=0/Grill; Rectifier No.=1; Routers=Not Exist; GPS=Not Exist; ADM=Not Exist; Modem=Not Exist; TX Indoor Qty.=1; TX Indoor Type=NEC I Paso Link 1000; BSC Qty=0; BSC Type=Not Exist; BTS Vendor=HUW; BTS QTY=1; 2G RF  Modules=12; 3G RF Modules=4; 4G RF Modules=3; Cabinet Type (Y/N) Y=Rectifier in Cabinet=Y  / Huawie Cabinet; Battery Type=SBS; Battery Strings Qty=4; Rectifier Type=Delta 2; No of Modules=5; TE Current C.B Rate=NA; Date Of Visit=44564
  - Row 5: #=1005; Site Code=8020DE; BSC Code=8018DE; BSC Name=BENHA01; OEG Site Name=QALIUB-EXCH; TE Name=QALIUB EXCHANGE; No . Of locations=1; Site Type=BSC; A/C Units=2; A/C Capacity=5HP , 5HP; Rectifier No.=2; Routers=Yes; GPS=Not Exist; ADM=Yes; Modem=Not Exist; TX Indoor Qty.=4; TX Indoor Type=NEC I Paso Link 1000 + NEC IPaso Link VR4; BSC Qty=1; BSC Type=NEC; BTS Vendor=No Radio in Site; BTS QTY=No Radio in Site; 2G RF  Modules=No Radio in Site; 3G RF Modules=No Radio in Site; 4G RF Modules=No Radio in Site; Cabinet Type (Y/N) Y=Rectifier in Cabinet=N; Battery Type=SBS; Battery Strings Qty=9; Rectifier Type=Delta 2 - Delta 2; No of Modules=6,5; TE Current C.B Rate=NA; Date Of Visit=44564

## Section 1: Coverage Summary Table

| Excel File | Sheet | Column | Domain Entity | Domain Property | Status |
|---|---|---|---|---|---|
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | ShortCode | Site | SiteCode | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Site OMC Name | Site | OMCName | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Site Code | Site | SiteCode | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | BSC Name | Site | BSCName | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Subcontractor | Site | Subcontractor | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Maintenance Area | Site | MaintenanceArea | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Region | Site | Region | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Subregion | Site | SubRegion | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Subcontractor Office | Office | Code/Name | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Announcement Date | Site | AnnouncementDate | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | On / Off  Air | Site | Status | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Site Coordinates X, Y | Site | Coordinates | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Address Detailes | Site | Address | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Sharing | SiteSharing | IsShared | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Tower | SiteTowerInfo | TowerType | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Radio Equipment Data | SiteTransmission | TransmissionType | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | DC Power | SitePowerSystem | BatteryType | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | AC Power | SitePowerSystem | PowerConfiguration | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Cooling System | SiteCoolingSystem | ACUnits/ACUnitsCount | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Fire Panel | SiteFireSafety | FirePanelType | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | ZTE Remote Monitoring System | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | General Data | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Assets Data Count | Status | Site | Status | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | # | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | Site Code | Site | SiteCode | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | BSC Code | Site | BSCCode | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | BSC Name | Site | BSCName | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | OEG Site Name | Site | Name/OMCName | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | TE Name | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | No . Of locations | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | Site Type | Site | SiteType | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | A/C Units | SiteCoolingSystem | ACUnits/ACUnitsCount | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | A/C Capacity | SiteCoolingSystem | ACUnits/ACUnitsCount | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | Rectifier No. | SitePowerSystem | RectifierModulesCount | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | Routers | SiteTransmission | HasALURouter | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | GPS | SiteTransmission | HasGPS | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | ADM | SiteTransmission | HasADM | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | Modem | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | TX Indoor Qty. | SiteTransmission | LinksCount | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | TX Indoor Type | MWLink | ODUType | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | BSC Qty | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | BSC Type | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | BTS Vendor | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | BTS QTY | SiteRadioEquipment | BTSCount | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | 2G RF  Modules | SiteRadioEquipment | TwoGModulesCount | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | 3G RF Modules | SiteRadioEquipment | ThreeGRadioModules | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | 4G RF Modules | SiteRadioEquipment | FourGModulesCount | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | Cabinet Type (Y/N) Y=Rectifier in Cabinet | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | Battery Type | SitePowerSystem | BatteryType/BatteryVoltage/BatteryAmpereHour | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | Battery Strings Qty | SitePowerSystem | BatteryStrings/BatteriesPerString | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | Rectifier Type | SitePowerSystem | RectifierBrand | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | No of Modules | SitePowerSystem | RectifierModulesCount | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | TE Current C.B Rate | SitePowerSystem | PowerMeterRate | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | Date Of Visit | Visit | ScheduledDate/CheckInTime | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Power Data | Contact Person | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Short Code | Site | SiteCode | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Name | Site | Name | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Date Of Visit | Visit | ScheduledDate/CheckInTime | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Sector Technology | SectorInfo | Technology | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Sector number | SectorInfo | SectorNumber | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Site Topology | SiteTowerInfo | TowerType | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Building Height (m) | SiteTowerInfo | Height | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Support Height (m) | SiteTowerInfo | Height | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Support ID | SiteTowerInfo | NumberOfMasts | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Azimuth | SectorInfo | Azimuth | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Antenna Type | SectorInfo | AntennaType | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Antenna Sharing | SiteSharing | IsShared | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | RRU/RF/RRH Solution | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Combiner/ Diplexer ID | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Mast Head Amplifier | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Bias-T | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | HBA(m) | SectorInfo | HeightAboveBase | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Bracket Tilt | SectorInfo | MechanicalTilt | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Elect Tilt | SectorInfo | ElectricalTilt | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Feeder Size / Cable Type | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Feeder Length | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Radio Data | Remarks | VisitChecklist/VisitIssue | Notes/Description | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Short Code | Site | SiteCode | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Local Site Name | Site | Name/OMCName | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Directions Site Name | MWLink | OppositeSite | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Directions Site Code | MWLink | OppositeSite | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Mast No. | SiteTowerInfo | NumberOfMasts | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | TX HBA | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | TX Azimuth | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Antenna Diameter [m] | MWLink | DishSizeCM | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Side Arm Height [m] | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Side Arm Diameter | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | IP Address | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Link Type | SiteTransmission | TransmissionType | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Link Model | MWLink | ODUType | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Band | MWLink | FrequencyBand | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Configuration | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Modulation | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Capacity [Mb/s] | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Tx Frequency [KHz] | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Rx Frequency [KHz] | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Tx Power [dBm] | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Rx Power [dBm] | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | ODU Model | MWLink | ODUType | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | ODU S/N | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Opposite site ODU S/N | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Antenna Reference | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Polarization | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site TX Data | Remarks | VisitChecklist/VisitIssue | Notes/Description | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Short Code | Site | SiteCode | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Name | Site | Name | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Site Host | SiteSharing | HostOperator | ? Mapped |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Host Code | SiteSharing | HostOperator | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Site Guests | SiteSharing | GuestOperators | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Topology | SiteTowerInfo | TowerType | ?? Partial |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX Enclosure | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Sharing Count Radio Antenna | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio Azimuth 1 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio HBA 1 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio Azimuth 2 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio HBA 2 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio Azimuth 3 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio HBA 3 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio Azimuth 4 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio HBA 4 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio Azimuth 5 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio HBA 5 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio Azimuth 6 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio HBA 6 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio Azimuth 7 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio HBA 7 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio Azimuth 8 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Radio HBA 8 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Sharing Count Tx Antenna | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX Azimuth 1 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX HBA 1 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX Azimuth 2 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX HBA 2 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX Azimuth 3 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX HBA 3 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX Azimuth 4 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX HBA 4 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX Azimuth 5 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX HBA 5 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX Azimuth 6 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX HBA 6 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX Azimuth 7 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX HBA 7 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX Azimuth 8 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX HBA 8 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX Azimuth 9 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | TX HBA 9 | - | - | ? Missing |
| Data collection from 1-1-2023 till 31-12-2023.xlsx | Site Sharing Data | Remarks | VisitChecklist/VisitIssue | Notes/Description | ?? Partial |
| Delta Sites (1).xlsx | Sheet2 | Short Code | Site | SiteCode | ?? Partial |
| Delta Sites (1).xlsx | Sheet2 | Site Name | Site | Name | ? Mapped |
| Delta Sites (1).xlsx | Sheet2 | Office | Office | Code/Name | ?? Partial |
| Delta Sites (1).xlsx | Sheet2 | Nodal Degree | - | - | ? Missing |
| Delta Sites (1).xlsx | Sheet2 | Rectifier Brand | SitePowerSystem | RectifierBrand | ?? Partial |
| Delta Sites (1).xlsx | Sheet2 | Battery Type/Volt/AH | SitePowerSystem | BatteryType/BatteryVoltage/BatteryAmpereHour | ?? Partial |
| Delta Sites (1).xlsx | Sheet2 | No of String | SitePowerSystem | BatteryStrings/BatteriesPerString | ?? Partial |
| Delta Sites (1).xlsx | Sheet2 | Batteries Status | - | - | ? Missing |
| Delta Sites (1).xlsx | Sheet2 | Announcement Date | Site | AnnouncementDate | ?? Partial |
| Delta Sites (1).xlsx | Sheet1 | Short Code | Site | SiteCode | ?? Partial |
| Delta Sites (1).xlsx | Sheet1 | Site Name | Site | Name | ? Mapped |
| Delta Sites (1).xlsx | Sheet1 | SC Office | Office | Code/Name | ?? Partial |
| Delta Sites (1).xlsx | Sheet1 | OZ | - | - | ? Missing |
| Delta Sites (1).xlsx | Sheet1 | Nodal Deg. | - | - | ? Missing |
| GH-BDT_BDT.xlsx | BDT sheet | [No row-1 headers] | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Power Alarm | [No row-1 headers] | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Config | [No row-1 headers] | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Week | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Ser | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Site Name | Site | Name | ? Mapped |
| GH-BDT_BDT.xlsx | Summary  | Short Code | Site | SiteCode | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | On Air Date | Site | AnnouncementDate | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | Nodal Degree | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | PLVD Value (LLVD For Huawei) adjusted after finishing the test | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Linked sites name codes | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Type | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Site Category (Shelter/OD/Grill) | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Power Source | SitePowerSystem | PowerConfiguration | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | # of BSC | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | BSC Type | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | # of BTS | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Type2 | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | # of GSM/MRFU/RF | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | # of DSC/MRFU/RF | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | # of MW | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | MW Type | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | # of SDH | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | # of ADM | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | # of Routers | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | AC1 Type | SiteCoolingSystem | ACUnits/ACUnitsCount | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | AC HP | SiteCoolingSystem | ACUnits/ACUnitsCount | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | AC2 Type | SiteCoolingSystem | ACUnits/ACUnitsCount | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | AC HP3 | SiteCoolingSystem | ACUnits/ACUnitsCount | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | 3G Type | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | No. Of 3G RF | SiteRadioEquipment | ThreeGRadioModules | ? Mapped |
| GH-BDT_BDT.xlsx | Summary  | 4G Type | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | No. Of 4G RF | SiteRadioEquipment | FourGModulesCount | ? Mapped |
| GH-BDT_BDT.xlsx | Summary  | Orange office | Office | Code/Name | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | Subcontractor | Site | Subcontractor | ? Mapped |
| GH-BDT_BDT.xlsx | Summary  | Office | Office | Code/Name | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | Area | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Network | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Rectifier Brand | SitePowerSystem | RectifierBrand | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | # of Modules | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Battery Brand | SitePowerSystem | BatteryType/BatteryVoltage/BatteryAmpereHour | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | Battery Volt | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Battery Ampere Hour | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | No of String | SitePowerSystem | BatteryStrings/BatteriesPerString | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | No of Batteries | SitePowerSystem | BatteryStrings/BatteriesPerString | ?? Partial |
| GH-BDT_BDT.xlsx | Summary  | Start Volt | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Start Amp | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Batteries Charnging current limit | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | End Volt | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | End Amp | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Discharge time( Mins) | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Reason for Test stop | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Test Date | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Reason for Repeated BDT | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Cap request # | - | - | ? Missing |
| GH-BDT_BDT.xlsx | Summary  | Comment | VisitChecklist/VisitIssue | Notes/Description | ?? Partial |
| GH-DE  Checklist.xlsx | site's reading | [No row-1 headers] | - | - | ? Missing |
| GH-DE  Checklist.xlsx | Common checklist | [No row-1 headers] | - | - | ? Missing |
| GH-DE  Checklist.xlsx | Panorama | Shelter panorama | VisitPhoto | PhotoCategory | ?? Partial |
| GH-DE  Checklist.xlsx | Sheet1 | [No row-1 headers] | - | - | ? Missing |
| GH-DE  Checklist.xlsx | Sheet2 | [No row-1 headers] | - | - | ? Missing |
| GH-DE  Checklist.xlsx | Tower Panorama | Tower panorama | VisitPhoto | PhotoCategory | ?? Partial |
| GH-DE  Checklist.xlsx | Before & after | [No row-1 headers] | - | - | ? Missing |
| GH-DE  Checklist.xlsx | Pending Res. | Pending Reserves | - | - | ? Missing |
| GH-DE  Checklist.xlsx | unused assets | unused assets | - | - | ? Missing |
| GH-DE  Checklist.xlsx | alarms capture | BTS capture ( Alarms ) | VisitPhoto | PhotoCategory | ?? Partial |
| GH-DE  Checklist.xlsx | alarms capture | 3G capture | VisitPhoto | PhotoCategory | ?? Partial |
| GH-DE  Checklist.xlsx | alarms capture | MW capture | VisitPhoto | PhotoCategory | ?? Partial |
| GH-DE  Checklist.xlsx | Audit matrix SQI | File | - | - | ? Missing |
| GH-DE  Checklist.xlsx | Audit matrix SQI | Network Audit Checklist (applicable on entire radio sites) | VisitChecklist | Category/ItemName/Status | ?? Partial |
| GH-DE Data Collection.xlsx | Site Sharing Data | Short Code | Site | SiteCode | ?? Partial |
| GH-DE Data Collection.xlsx | Site Sharing Data | Name | Site | Name | ? Mapped |
| GH-DE Data Collection.xlsx | Site Sharing Data | Site Host | SiteSharing | HostOperator | ? Mapped |
| GH-DE Data Collection.xlsx | Site Sharing Data | Host Code | SiteSharing | HostOperator | ?? Partial |
| GH-DE Data Collection.xlsx | Site Sharing Data | Site Guests | SiteSharing | GuestOperators | ?? Partial |
| GH-DE Data Collection.xlsx | Site Sharing Data | Topology | SiteTowerInfo | TowerType | ?? Partial |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX Enclosure | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Sharing Count Radio Antenna | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio Azimuth 1 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio HBA 1 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio Azimuth 2 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio HBA 2 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio Azimuth 3 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio HBA 3 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio Azimuth 4 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio HBA 4 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio Azimuth 5 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio HBA 5 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio Azimuth 6 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio HBA 6 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio Azimuth 7 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio HBA 7 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio Azimuth 8 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Radio HBA 8 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Sharing Count Tx Antenna | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX Azimuth 1 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX HBA 1 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX Azimuth 2 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX HBA 2 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX Azimuth 3 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX HBA 3 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX Azimuth 4 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX HBA 4 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX Azimuth 5 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX HBA 5 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX Azimuth 6 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX HBA 6 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX Azimuth 7 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX HBA 7 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX Azimuth 8 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX HBA 8 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX Azimuth 9 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | TX HBA 9 | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Sharing Data | Remarks | VisitChecklist/VisitIssue | Notes/Description | ?? Partial |
| GH-DE Data Collection.xlsx | Site TX Data | Short Code | Site | SiteCode | ?? Partial |
| GH-DE Data Collection.xlsx | Site TX Data | Local Site Name | Site | Name/OMCName | ?? Partial |
| GH-DE Data Collection.xlsx | Site TX Data | Directions Site Name | MWLink | OppositeSite | ?? Partial |
| GH-DE Data Collection.xlsx | Site TX Data | Directions Site Code | MWLink | OppositeSite | ?? Partial |
| GH-DE Data Collection.xlsx | Site TX Data | Mast No. | SiteTowerInfo | NumberOfMasts | ?? Partial |
| GH-DE Data Collection.xlsx | Site TX Data | TX HBA | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | TX Azimuth | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Antenna Diameter [m] | MWLink | DishSizeCM | ?? Partial |
| GH-DE Data Collection.xlsx | Site TX Data | Side Arm Height [m] | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Side Arm Diameter | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | IP Address | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Link Type | SiteTransmission | TransmissionType | ?? Partial |
| GH-DE Data Collection.xlsx | Site TX Data | Link Model | MWLink | ODUType | ?? Partial |
| GH-DE Data Collection.xlsx | Site TX Data | Band | MWLink | FrequencyBand | ? Mapped |
| GH-DE Data Collection.xlsx | Site TX Data | Configuration | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Modulation | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Capacity [Mb/s] | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Tx Frequency [KHz] | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Rx Frequency [KHz] | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Tx Power [dBm] | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Rx Power [dBm] | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | ODU Model | MWLink | ODUType | ?? Partial |
| GH-DE Data Collection.xlsx | Site TX Data | ODU S/N | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Opposite site ODU S/N | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Antenna Reference | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Polarization | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site TX Data | Remarks | VisitChecklist/VisitIssue | Notes/Description | ?? Partial |
| GH-DE Data Collection.xlsx | Site Radio Data | Short Code | Site | SiteCode | ?? Partial |
| GH-DE Data Collection.xlsx | Site Radio Data | Name | Site | Name | ? Mapped |
| GH-DE Data Collection.xlsx | Site Radio Data | Date Of Visit | Visit | ScheduledDate/CheckInTime | ?? Partial |
| GH-DE Data Collection.xlsx | Site Radio Data | Sector Technology | SectorInfo | Technology | ?? Partial |
| GH-DE Data Collection.xlsx | Site Radio Data | Sector number | SectorInfo | SectorNumber | ? Mapped |
| GH-DE Data Collection.xlsx | Site Radio Data | Site Topology | SiteTowerInfo | TowerType | ?? Partial |
| GH-DE Data Collection.xlsx | Site Radio Data | Building Height (m) | SiteTowerInfo | Height | ?? Partial |
| GH-DE Data Collection.xlsx | Site Radio Data | Support Height (m) | SiteTowerInfo | Height | ?? Partial |
| GH-DE Data Collection.xlsx | Site Radio Data | Support ID | SiteTowerInfo | NumberOfMasts | ?? Partial |
| GH-DE Data Collection.xlsx | Site Radio Data | Azimuth | SectorInfo | Azimuth | ? Mapped |
| GH-DE Data Collection.xlsx | Site Radio Data | Antenna Type | SectorInfo | AntennaType | ? Mapped |
| GH-DE Data Collection.xlsx | Site Radio Data | Antenna Sharing | SiteSharing | IsShared | ?? Partial |
| GH-DE Data Collection.xlsx | Site Radio Data | RRU/RF/RRH Solution | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Radio Data | Combiner/ Diplexer ID | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Radio Data | Mast Head Amplifier | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Radio Data | Bias-T | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Radio Data | HBA(m) | SectorInfo | HeightAboveBase | ? Mapped |
| GH-DE Data Collection.xlsx | Site Radio Data | Bracket Tilt | SectorInfo | MechanicalTilt | ? Mapped |
| GH-DE Data Collection.xlsx | Site Radio Data | Elect Tilt | SectorInfo | ElectricalTilt | ? Mapped |
| GH-DE Data Collection.xlsx | Site Radio Data | Feeder Size / Cable Type | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Radio Data | Feeder Length | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Radio Data | Remarks | VisitChecklist/VisitIssue | Notes/Description | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | # | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Power Data | Site Code | Site | SiteCode | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | BSC Code | Site | BSCCode | ? Mapped |
| GH-DE Data Collection.xlsx | Power Data | BSC Name | Site | BSCName | ? Mapped |
| GH-DE Data Collection.xlsx | Power Data | OEG Site Name | Site | Name/OMCName | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | TE Name | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Power Data | No . Of locations | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Power Data | Site Type | Site | SiteType | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | A/C Units | SiteCoolingSystem | ACUnits/ACUnitsCount | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | A/C Capacity | SiteCoolingSystem | ACUnits/ACUnitsCount | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | Rectifier No. | SitePowerSystem | RectifierModulesCount | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | Routers | SiteTransmission | HasALURouter | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | GPS | SiteTransmission | HasGPS | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | ADM | SiteTransmission | HasADM | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | Modem | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Power Data | TX Indoor Qty. | SiteTransmission | LinksCount | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | TX Indoor Type | MWLink | ODUType | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | BSC Qty | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Power Data | BSC Type | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Power Data | BTS Vendor | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Power Data | BTS QTY | SiteRadioEquipment | BTSCount | ? Mapped |
| GH-DE Data Collection.xlsx | Power Data | 2G RF  Modules | SiteRadioEquipment | TwoGModulesCount | ? Mapped |
| GH-DE Data Collection.xlsx | Power Data | 3G RF Modules | SiteRadioEquipment | ThreeGRadioModules | ? Mapped |
| GH-DE Data Collection.xlsx | Power Data | 4G RF Modules | SiteRadioEquipment | FourGModulesCount | ? Mapped |
| GH-DE Data Collection.xlsx | Power Data | Cabinet Type (Y/N) Y=Rectifier in Cabinet | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Power Data | Battery Type | SitePowerSystem | BatteryType/BatteryVoltage/BatteryAmpereHour | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | Battery Strings Qty | SitePowerSystem | BatteryStrings/BatteriesPerString | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | Rectifier Type | SitePowerSystem | RectifierBrand | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | No of Modules | SitePowerSystem | RectifierModulesCount | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | TE Current C.B Rate | SitePowerSystem | PowerMeterRate | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | Date Of Visit | Visit | ScheduledDate/CheckInTime | ?? Partial |
| GH-DE Data Collection.xlsx | Power Data | Contact Person | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Assets Data Count | ShortCode | Site | SiteCode | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Site OMC Name | Site | OMCName | ? Mapped |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Site Code | Site | SiteCode | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | BSC Name | Site | BSCName | ? Mapped |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Subcontractor | Site | Subcontractor | ? Mapped |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Maintenance Area | Site | MaintenanceArea | ? Mapped |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Region | Site | Region | ? Mapped |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Subregion | Site | SubRegion | ? Mapped |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Subcontractor Office | Office | Code/Name | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Announcement Date | Site | AnnouncementDate | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | On / Off  Air | Site | Status | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Site Coordinates X, Y | Site | Coordinates | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Address Detailes | Site | Address | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Sharing | SiteSharing | IsShared | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Tower | SiteTowerInfo | TowerType | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Radio Equipment Data | SiteTransmission | TransmissionType | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | DC Power | SitePowerSystem | BatteryType | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | AC Power | SitePowerSystem | PowerConfiguration | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Cooling System | SiteCoolingSystem | ACUnits/ACUnitsCount | ?? Partial |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Fire Panel | SiteFireSafety | FirePanelType | ? Mapped |
| GH-DE Data Collection.xlsx | Site Assets Data Count | ZTE Remote Monitoring System | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Assets Data Count | General Data | - | - | ? Missing |
| GH-DE Data Collection.xlsx | Site Assets Data Count | Status | Site | Status | ?? Partial |
| GH-DE Data Collection.xlsx | RF Status | Site code | Site | SiteCode | ?? Partial |
| GH-DE Data Collection.xlsx | RF Status | site name | Site | Name | ? Mapped |
| GH-DE Data Collection.xlsx | RF Status | total RF In site | - | - | ? Missing |
| GH-DE Data Collection.xlsx | RF Status | RF on tower Number | - | - | ? Missing |
| GH-DE Data Collection.xlsx | RF Status | RF on ground Number | - | - | ? Missing |
| GH-DE Data Collection.xlsx | RF Status | Number RF sector carry | - | - | ? Missing |
| GH-DE Data Collection.xlsx | RF Status | band for RF on tower | - | - | ? Missing |
| GH-DE Data Collection.xlsx | RF Status | band for RF on ground | - | - | ? Missing |
| GH-DE Data Collection.xlsx | RF Status | comment | VisitChecklist/VisitIssue | Notes/Description | ?? Partial |
| Power Data.xlsx | Power Data | # | - | - | ? Missing |
| Power Data.xlsx | Power Data | Site Code | Site | SiteCode | ?? Partial |
| Power Data.xlsx | Power Data | BSC Code | Site | BSCCode | ? Mapped |
| Power Data.xlsx | Power Data | BSC Name | Site | BSCName | ? Mapped |
| Power Data.xlsx | Power Data | OEG Site Name | Site | Name/OMCName | ?? Partial |
| Power Data.xlsx | Power Data | TE Name | - | - | ? Missing |
| Power Data.xlsx | Power Data | No . Of locations | - | - | ? Missing |
| Power Data.xlsx | Power Data | Site Type | Site | SiteType | ?? Partial |
| Power Data.xlsx | Power Data | A/C Units | SiteCoolingSystem | ACUnits/ACUnitsCount | ?? Partial |
| Power Data.xlsx | Power Data | A/C Capacity | SiteCoolingSystem | ACUnits/ACUnitsCount | ?? Partial |
| Power Data.xlsx | Power Data | Rectifier No. | SitePowerSystem | RectifierModulesCount | ?? Partial |
| Power Data.xlsx | Power Data | Routers | SiteTransmission | HasALURouter | ?? Partial |
| Power Data.xlsx | Power Data | GPS | SiteTransmission | HasGPS | ?? Partial |
| Power Data.xlsx | Power Data | ADM | SiteTransmission | HasADM | ?? Partial |
| Power Data.xlsx | Power Data | Modem | - | - | ? Missing |
| Power Data.xlsx | Power Data | TX Indoor Qty. | SiteTransmission | LinksCount | ?? Partial |
| Power Data.xlsx | Power Data | TX Indoor Type | MWLink | ODUType | ?? Partial |
| Power Data.xlsx | Power Data | BSC Qty | - | - | ? Missing |
| Power Data.xlsx | Power Data | BSC Type | - | - | ? Missing |
| Power Data.xlsx | Power Data | BTS Vendor | - | - | ? Missing |
| Power Data.xlsx | Power Data | BTS QTY | SiteRadioEquipment | BTSCount | ? Mapped |
| Power Data.xlsx | Power Data | 2G RF  Modules | SiteRadioEquipment | TwoGModulesCount | ? Mapped |
| Power Data.xlsx | Power Data | 3G RF Modules | SiteRadioEquipment | ThreeGRadioModules | ? Mapped |
| Power Data.xlsx | Power Data | 4G RF Modules | SiteRadioEquipment | FourGModulesCount | ? Mapped |
| Power Data.xlsx | Power Data | Cabinet Type (Y/N) Y=Rectifier in Cabinet | - | - | ? Missing |
| Power Data.xlsx | Power Data | Battery Type | SitePowerSystem | BatteryType/BatteryVoltage/BatteryAmpereHour | ?? Partial |
| Power Data.xlsx | Power Data | Battery Strings Qty | SitePowerSystem | BatteryStrings/BatteriesPerString | ?? Partial |
| Power Data.xlsx | Power Data | Rectifier Type | SitePowerSystem | RectifierBrand | ?? Partial |
| Power Data.xlsx | Power Data | No of Modules | SitePowerSystem | RectifierModulesCount | ?? Partial |
| Power Data.xlsx | Power Data | TE Current C.B Rate | SitePowerSystem | PowerMeterRate | ?? Partial |
| Power Data.xlsx | Power Data | Date Of Visit | Visit | ScheduledDate/CheckInTime | ?? Partial |
| Power Data.xlsx | Power Data | Contact Person | - | - | ? Missing |

## Section 2: Missing Fields

- Excel column name: `ShortCode`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `ShortCode` to `Site.SiteCode`.
  Priority: **Low**
- Excel column name: `Site OMC Name`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Site OMC Name`.
  Priority: **Low**
- Excel column name: `Site Code`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `BSC Name`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `BSC Name`.
  Priority: **Low**
- Excel column name: `Subcontractor`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Subcontractor`.
  Priority: **Low**
- Excel column name: `Maintenance Area`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Maintenance Area`.
  Priority: **Low**
- Excel column name: `Region`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Region`.
  Priority: **High**
- Excel column name: `Subregion`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Subregion`.
  Priority: **High**
- Excel column name: `Subcontractor Office`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Subcontractor Office` to `Office.Code/Name`.
  Priority: **Low**
- Excel column name: `Announcement Date`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Announcement Date` to `Site.AnnouncementDate`.
  Priority: **High**
- Excel column name: `On / Off  Air`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `On / Off  Air` to `Site.Status`.
  Priority: **Low**
- Excel column name: `Site Coordinates X, Y`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Coordinates X, Y` to `Site.Coordinates`.
  Priority: **Low**
- Excel column name: `Address Detailes`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Address Detailes` to `Site.Address`.
  Priority: **Low**
- Excel column name: `Sharing`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Sharing` to `SiteSharing.IsShared`.
  Priority: **Low**
- Excel column name: `Tower`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Tower` to `SiteTowerInfo.TowerType`.
  Priority: **High**
- Excel column name: `Radio Equipment Data`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Radio Equipment Data` to `SiteTransmission.TransmissionType`.
  Priority: **Low**
- Excel column name: `DC Power`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **High**
- Excel column name: `AC Power`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **High**
- Excel column name: `Cooling System`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Cooling System` to `SiteCoolingSystem.ACUnits/ACUnitsCount`.
  Priority: **Low**
- Excel column name: `Fire Panel`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Fire Panel`.
  Priority: **Low**
- Excel column name: `ZTE Remote Monitoring System`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add remote monitoring capability fields under Site.
  Priority: **Low**
- Excel column name: `General Data`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `General Data`.
  Priority: **Low**
- Excel column name: `Status`
  Sheet name: `Site Assets Data Count`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Status` to `Site.Status`.
  Priority: **High**
- Excel column name: `#`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `#`.
  Priority: **Low**
- Excel column name: `Site Code`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `BSC Code`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `BSC Code`.
  Priority: **Low**
- Excel column name: `BSC Name`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `BSC Name`.
  Priority: **Low**
- Excel column name: `OEG Site Name`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `OEG Site Name` to `Site.Name/OMCName`.
  Priority: **Low**
- Excel column name: `TE Name`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TE Name`.
  Priority: **Low**
- Excel column name: `No . Of locations`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `No . Of locations`.
  Priority: **Low**
- Excel column name: `Site Type`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Type` to `Site.SiteType`.
  Priority: **High**
- Excel column name: `A/C Units`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `A/C Units` to `SiteCoolingSystem.ACUnits/ACUnitsCount`.
  Priority: **Low**
- Excel column name: `A/C Capacity`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Rectifier No.`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Rectifier No.` to `SitePowerSystem.RectifierModulesCount`.
  Priority: **High**
- Excel column name: `Routers`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Routers` to `SiteTransmission.HasALURouter`.
  Priority: **Low**
- Excel column name: `GPS`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `GPS` to `SiteTransmission.HasGPS`.
  Priority: **Low**
- Excel column name: `ADM`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `ADM` to `SiteTransmission.HasADM`.
  Priority: **Low**
- Excel column name: `Modem`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Modem`.
  Priority: **Low**
- Excel column name: `TX Indoor Qty.`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `TX Indoor Qty.` to `SiteTransmission.LinksCount`.
  Priority: **Low**
- Excel column name: `TX Indoor Type`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `TX Indoor Type` to `MWLink.ODUType`.
  Priority: **Low**
- Excel column name: `BSC Qty`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `BSC Qty`.
  Priority: **Low**
- Excel column name: `BSC Type`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `BSC Type`.
  Priority: **Low**
- Excel column name: `BTS Vendor`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `BTS Vendor`.
  Priority: **Low**
- Excel column name: `BTS QTY`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `BTS QTY`.
  Priority: **Low**
- Excel column name: `2G RF  Modules`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `3G RF Modules`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `4G RF Modules`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Cabinet Type (Y/N) Y=Rectifier in Cabinet`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add cabinet metadata fields to `SitePowerSystem`.
  Priority: **High**
- Excel column name: `Battery Type`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Battery Type` to `SitePowerSystem.BatteryType/BatteryVoltage/BatteryAmpereHour`.
  Priority: **High**
- Excel column name: `Battery Strings Qty`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Battery Strings Qty` to `SitePowerSystem.BatteryStrings/BatteriesPerString`.
  Priority: **High**
- Excel column name: `Rectifier Type`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Rectifier Type` to `SitePowerSystem.RectifierBrand`.
  Priority: **High**
- Excel column name: `No of Modules`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `TE Current C.B Rate`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `TE Current C.B Rate` to `SitePowerSystem.PowerMeterRate`.
  Priority: **Low**
- Excel column name: `Date Of Visit`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Date Of Visit` to `Visit.ScheduledDate/CheckInTime`.
  Priority: **High**
- Excel column name: `Contact Person`
  Sheet name: `Power Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Contact Person`.
  Priority: **Low**
- Excel column name: `Short Code`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Short Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `Name`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Name`.
  Priority: **Low**
- Excel column name: `Date Of Visit`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Date Of Visit` to `Visit.ScheduledDate/CheckInTime`.
  Priority: **High**
- Excel column name: `Sector Technology`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Sector Technology` to `SectorInfo.Technology`.
  Priority: **Low**
- Excel column name: `Sector number`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Sector number`.
  Priority: **Low**
- Excel column name: `Site Topology`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Topology` to `SiteTowerInfo.TowerType`.
  Priority: **Low**
- Excel column name: `Building Height (m)`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Building Height (m)` to `SiteTowerInfo.Height`.
  Priority: **Low**
- Excel column name: `Support Height (m)`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Support Height (m)` to `SiteTowerInfo.Height`.
  Priority: **Low**
- Excel column name: `Support ID`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Support ID` to `SiteTowerInfo.NumberOfMasts`.
  Priority: **Low**
- Excel column name: `Azimuth`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Azimuth`.
  Priority: **Low**
- Excel column name: `Antenna Type`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Antenna Type`.
  Priority: **Low**
- Excel column name: `Antenna Sharing`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Antenna Sharing` to `SiteSharing.IsShared`.
  Priority: **Low**
- Excel column name: `RRU/RF/RRH Solution`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `RRU/RF/RRH Solution`.
  Priority: **Medium**
- Excel column name: `Combiner/ Diplexer ID`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Combiner/ Diplexer ID`.
  Priority: **Low**
- Excel column name: `Mast Head Amplifier`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Mast Head Amplifier`.
  Priority: **Low**
- Excel column name: `Bias-T`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Bias-T`.
  Priority: **Low**
- Excel column name: `HBA(m)`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `HBA(m)`.
  Priority: **Low**
- Excel column name: `Bracket Tilt`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Bracket Tilt`.
  Priority: **Low**
- Excel column name: `Elect Tilt`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Elect Tilt`.
  Priority: **Low**
- Excel column name: `Feeder Size / Cable Type`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Feeder Size / Cable Type`.
  Priority: **Low**
- Excel column name: `Feeder Length`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Feeder Length`.
  Priority: **Low**
- Excel column name: `Remarks`
  Sheet name: `Site Radio Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Remarks` to `VisitChecklist/VisitIssue.Notes/Description`.
  Priority: **Low**
- Excel column name: `Short Code`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Short Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `Local Site Name`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Local Site Name` to `Site.Name/OMCName`.
  Priority: **Low**
- Excel column name: `Directions Site Name`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Directions Site Name` to `MWLink.OppositeSite`.
  Priority: **Low**
- Excel column name: `Directions Site Code`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Directions Site Code` to `MWLink.OppositeSite`.
  Priority: **High**
- Excel column name: `Mast No.`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Mast No.` to `SiteTowerInfo.NumberOfMasts`.
  Priority: **Low**
- Excel column name: `TX HBA`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA`.
  Priority: **Low**
- Excel column name: `TX Azimuth`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth`.
  Priority: **Low**
- Excel column name: `Antenna Diameter [m]`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Antenna Diameter [m]` to `MWLink.DishSizeCM`.
  Priority: **Low**
- Excel column name: `Side Arm Height [m]`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Side Arm Height [m]`.
  Priority: **Low**
- Excel column name: `Side Arm Diameter`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Side Arm Diameter`.
  Priority: **Low**
- Excel column name: `IP Address`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Link Type`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Link Type` to `SiteTransmission.TransmissionType`.
  Priority: **Low**
- Excel column name: `Link Model`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Link Model` to `MWLink.ODUType`.
  Priority: **Low**
- Excel column name: `Band`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Band`.
  Priority: **Low**
- Excel column name: `Configuration`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Configuration`.
  Priority: **Low**
- Excel column name: `Modulation`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Capacity [Mb/s]`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Tx Frequency [KHz]`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Rx Frequency [KHz]`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Tx Power [dBm]`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **High**
- Excel column name: `Rx Power [dBm]`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **High**
- Excel column name: `ODU Model`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `ODU S/N`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Opposite site ODU S/N`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Antenna Reference`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Antenna Reference`.
  Priority: **Low**
- Excel column name: `Polarization`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Remarks`
  Sheet name: `Site TX Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Remarks` to `VisitChecklist/VisitIssue.Notes/Description`.
  Priority: **Low**
- Excel column name: `Short Code`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Short Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `Name`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Name`.
  Priority: **Low**
- Excel column name: `Site Host`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Site Host`.
  Priority: **Low**
- Excel column name: `Host Code`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Host Code` to `SiteSharing.HostOperator`.
  Priority: **Low**
- Excel column name: `Site Guests`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Guests` to `SiteSharing.GuestOperators`.
  Priority: **Low**
- Excel column name: `Topology`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Topology` to `SiteTowerInfo.TowerType`.
  Priority: **Low**
- Excel column name: `TX Enclosure`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX Enclosure`.
  Priority: **Low**
- Excel column name: `Sharing Count Radio Antenna`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Sharing Count Radio Antenna`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 1`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 1`.
  Priority: **Low**
- Excel column name: `Radio HBA 1`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 1`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 2`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 2`.
  Priority: **Low**
- Excel column name: `Radio HBA 2`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 2`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 3`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 3`.
  Priority: **Low**
- Excel column name: `Radio HBA 3`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 3`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 4`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 4`.
  Priority: **Low**
- Excel column name: `Radio HBA 4`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 4`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 5`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 5`.
  Priority: **Low**
- Excel column name: `Radio HBA 5`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 5`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 6`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 6`.
  Priority: **Low**
- Excel column name: `Radio HBA 6`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 6`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 7`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 7`.
  Priority: **Low**
- Excel column name: `Radio HBA 7`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 7`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 8`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 8`.
  Priority: **Low**
- Excel column name: `Radio HBA 8`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 8`.
  Priority: **Low**
- Excel column name: `Sharing Count Tx Antenna`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `Sharing Count Tx Antenna`.
  Priority: **Low**
- Excel column name: `TX Azimuth 1`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 1`.
  Priority: **Low**
- Excel column name: `TX HBA 1`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 1`.
  Priority: **Low**
- Excel column name: `TX Azimuth 2`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 2`.
  Priority: **Low**
- Excel column name: `TX HBA 2`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 2`.
  Priority: **Low**
- Excel column name: `TX Azimuth 3`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 3`.
  Priority: **Low**
- Excel column name: `TX HBA 3`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 3`.
  Priority: **Low**
- Excel column name: `TX Azimuth 4`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 4`.
  Priority: **Low**
- Excel column name: `TX HBA 4`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 4`.
  Priority: **Low**
- Excel column name: `TX Azimuth 5`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 5`.
  Priority: **Low**
- Excel column name: `TX HBA 5`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 5`.
  Priority: **Low**
- Excel column name: `TX Azimuth 6`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 6`.
  Priority: **Low**
- Excel column name: `TX HBA 6`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 6`.
  Priority: **Low**
- Excel column name: `TX Azimuth 7`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 7`.
  Priority: **Low**
- Excel column name: `TX HBA 7`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 7`.
  Priority: **Low**
- Excel column name: `TX Azimuth 8`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 8`.
  Priority: **Low**
- Excel column name: `TX HBA 8`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 8`.
  Priority: **Low**
- Excel column name: `TX Azimuth 9`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 9`.
  Priority: **Low**
- Excel column name: `TX HBA 9`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 9`.
  Priority: **Low**
- Excel column name: `Remarks`
  Sheet name: `Site Sharing Data`
  File name: `Data collection from 1-1-2023 till 31-12-2023.xlsx`
  Recommended domain change: Refine mapping/validation for `Remarks` to `VisitChecklist/VisitIssue.Notes/Description`.
  Priority: **Low**
- Excel column name: `Short Code`
  Sheet name: `Sheet2`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Refine mapping/validation for `Short Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `Site Name`
  Sheet name: `Sheet2`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Add new field/entity for `Site Name`.
  Priority: **Low**
- Excel column name: `Office`
  Sheet name: `Sheet2`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Refine mapping/validation for `Office` to `Office.Code/Name`.
  Priority: **Low**
- Excel column name: `Nodal Degree`
  Sheet name: `Sheet2`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Add nodal/connectivity degree fields or a dedicated topology entity.
  Priority: **Medium**
- Excel column name: `Rectifier Brand`
  Sheet name: `Sheet2`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Refine mapping/validation for `Rectifier Brand` to `SitePowerSystem.RectifierBrand`.
  Priority: **High**
- Excel column name: `Battery Type/Volt/AH`
  Sheet name: `Sheet2`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Refine mapping/validation for `Battery Type/Volt/AH` to `SitePowerSystem.BatteryType/BatteryVoltage/BatteryAmpereHour`.
  Priority: **High**
- Excel column name: `No of String`
  Sheet name: `Sheet2`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Refine mapping/validation for `No of String` to `SitePowerSystem.BatteryStrings/BatteriesPerString`.
  Priority: **Low**
- Excel column name: `Batteries Status`
  Sheet name: `Sheet2`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Add new field/entity for `Batteries Status`.
  Priority: **High**
- Excel column name: `Announcement Date`
  Sheet name: `Sheet2`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Refine mapping/validation for `Announcement Date` to `Site.AnnouncementDate`.
  Priority: **High**
- Excel column name: `Short Code`
  Sheet name: `Sheet1`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Refine mapping/validation for `Short Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `Site Name`
  Sheet name: `Sheet1`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Add new field/entity for `Site Name`.
  Priority: **Low**
- Excel column name: `SC Office`
  Sheet name: `Sheet1`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Refine mapping/validation for `SC Office` to `Office.Code/Name`.
  Priority: **Low**
- Excel column name: `OZ`
  Sheet name: `Sheet1`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Add new field/entity for `OZ`.
  Priority: **Low**
- Excel column name: `Nodal Deg.`
  Sheet name: `Sheet1`
  File name: `Delta Sites (1).xlsx`
  Recommended domain change: Add nodal/connectivity degree fields or a dedicated topology entity.
  Priority: **Medium**
- Excel column name: `[No row-1 headers]`
  Sheet name: `BDT sheet`
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `[No row-1 headers]`.
  Priority: **Low**
- Excel column name: `[No row-1 headers]`
  Sheet name: `Power Alarm`
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `[No row-1 headers]`.
  Priority: **Low**
- Excel column name: `[No row-1 headers]`
  Sheet name: `Config`
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `[No row-1 headers]`.
  Priority: **Low**
- Excel column name: `Week`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Week`.
  Priority: **Low**
- Excel column name: `Ser`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Ser`.
  Priority: **Low**
- Excel column name: `Site Name`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Site Name`.
  Priority: **Low**
- Excel column name: `Short Code`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `Short Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `On Air Date`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `On Air Date` to `Site.AnnouncementDate`.
  Priority: **High**
- Excel column name: `Nodal Degree`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add nodal/connectivity degree fields or a dedicated topology entity.
  Priority: **Medium**
- Excel column name: `PLVD Value (LLVD For Huawei) adjusted after finishing the test`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Introduce `BatteryDischargeTest` aggregate/value object for BDT-specific metrics.
  Priority: **Low**
- Excel column name: `Linked sites name codes`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Linked sites name codes`.
  Priority: **Low**
- Excel column name: `Type`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Type`.
  Priority: **Low**
- Excel column name: `Site Category (Shelter/OD/Grill)`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Site Category (Shelter/OD/Grill)`.
  Priority: **Low**
- Excel column name: `Power Source`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **High**
- Excel column name: `# of BSC`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `# of BSC`.
  Priority: **Low**
- Excel column name: `BSC Type`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `BSC Type`.
  Priority: **Low**
- Excel column name: `# of BTS`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `# of BTS`.
  Priority: **Low**
- Excel column name: `Type2`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Type2`.
  Priority: **Low**
- Excel column name: `# of GSM/MRFU/RF`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `# of GSM/MRFU/RF`.
  Priority: **Medium**
- Excel column name: `# of DSC/MRFU/RF`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `# of DSC/MRFU/RF`.
  Priority: **Medium**
- Excel column name: `# of MW`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `# of MW`.
  Priority: **Low**
- Excel column name: `MW Type`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `MW Type`.
  Priority: **Low**
- Excel column name: `# of SDH`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `# of SDH`.
  Priority: **Low**
- Excel column name: `# of ADM`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `# of ADM`.
  Priority: **Low**
- Excel column name: `# of Routers`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `# of Routers`.
  Priority: **Low**
- Excel column name: `AC1 Type`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `AC1 Type` to `SiteCoolingSystem.ACUnits/ACUnitsCount`.
  Priority: **Low**
- Excel column name: `AC HP`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `AC HP` to `SiteCoolingSystem.ACUnits/ACUnitsCount`.
  Priority: **Low**
- Excel column name: `AC2 Type`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `AC2 Type` to `SiteCoolingSystem.ACUnits/ACUnitsCount`.
  Priority: **Low**
- Excel column name: `AC HP3`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `AC HP3` to `SiteCoolingSystem.ACUnits/ACUnitsCount`.
  Priority: **Low**
- Excel column name: `3G Type`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `3G Type`.
  Priority: **Low**
- Excel column name: `No. Of 3G RF`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `No. Of 3G RF`.
  Priority: **Medium**
- Excel column name: `4G Type`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `4G Type`.
  Priority: **Low**
- Excel column name: `No. Of 4G RF`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `No. Of 4G RF`.
  Priority: **Medium**
- Excel column name: `Orange office`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `Orange office` to `Office.Code/Name`.
  Priority: **Low**
- Excel column name: `Subcontractor`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Subcontractor`.
  Priority: **Low**
- Excel column name: `Office`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `Office` to `Office.Code/Name`.
  Priority: **Low**
- Excel column name: `Area`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Area`.
  Priority: **Low**
- Excel column name: `Network`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Network`.
  Priority: **Low**
- Excel column name: `Rectifier Brand`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `Rectifier Brand` to `SitePowerSystem.RectifierBrand`.
  Priority: **High**
- Excel column name: `# of Modules`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Battery Brand`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `Battery Brand` to `SitePowerSystem.BatteryType/BatteryVoltage/BatteryAmpereHour`.
  Priority: **High**
- Excel column name: `Battery Volt`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Battery Volt`.
  Priority: **High**
- Excel column name: `Battery Ampere Hour`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Battery Ampere Hour`.
  Priority: **High**
- Excel column name: `No of String`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `No of String` to `SitePowerSystem.BatteryStrings/BatteriesPerString`.
  Priority: **Low**
- Excel column name: `No of Batteries`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `No of Batteries` to `SitePowerSystem.BatteryStrings/BatteriesPerString`.
  Priority: **Low**
- Excel column name: `Start Volt`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Start Volt`.
  Priority: **Low**
- Excel column name: `Start Amp`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Start Amp`.
  Priority: **Low**
- Excel column name: `Batteries Charnging current limit`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Batteries Charnging current limit`.
  Priority: **Low**
- Excel column name: `End Volt`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `End Volt`.
  Priority: **Low**
- Excel column name: `End Amp`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `End Amp`.
  Priority: **Low**
- Excel column name: `Discharge time( Mins)`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Introduce `BatteryDischargeTest` aggregate/value object for BDT-specific metrics.
  Priority: **Low**
- Excel column name: `Reason for Test stop`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Reason for Test stop`.
  Priority: **Low**
- Excel column name: `Test Date`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Test Date`.
  Priority: **High**
- Excel column name: `Reason for Repeated BDT`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Reason for Repeated BDT`.
  Priority: **Low**
- Excel column name: `Cap request #`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Add new field/entity for `Cap request #`.
  Priority: **Low**
- Excel column name: `Comment`
  Sheet name: `Summary `
  File name: `GH-BDT_BDT.xlsx`
  Recommended domain change: Refine mapping/validation for `Comment` to `VisitChecklist/VisitIssue.Notes/Description`.
  Priority: **Low**
- Excel column name: `[No row-1 headers]`
  Sheet name: `site's reading`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Add new field/entity for `[No row-1 headers]`.
  Priority: **Low**
- Excel column name: `[No row-1 headers]`
  Sheet name: `Common checklist`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Add new field/entity for `[No row-1 headers]`.
  Priority: **Low**
- Excel column name: `Shelter panorama`
  Sheet name: `Panorama`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Refine mapping/validation for `Shelter panorama` to `VisitPhoto.PhotoCategory`.
  Priority: **Low**
- Excel column name: `[No row-1 headers]`
  Sheet name: `Sheet1`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Add new field/entity for `[No row-1 headers]`.
  Priority: **Low**
- Excel column name: `[No row-1 headers]`
  Sheet name: `Sheet2`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Add new field/entity for `[No row-1 headers]`.
  Priority: **Low**
- Excel column name: `Tower panorama`
  Sheet name: `Tower Panorama`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Refine mapping/validation for `Tower panorama` to `VisitPhoto.PhotoCategory`.
  Priority: **High**
- Excel column name: `[No row-1 headers]`
  Sheet name: `Before & after`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Add new field/entity for `[No row-1 headers]`.
  Priority: **Low**
- Excel column name: `Pending Reserves`
  Sheet name: `Pending Res.`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Add new field/entity for `Pending Reserves`.
  Priority: **Low**
- Excel column name: `unused assets`
  Sheet name: `unused assets`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Add `UnusedAsset` entity linked to Site/Visit.
  Priority: **Low**
- Excel column name: `BTS capture ( Alarms )`
  Sheet name: `alarms capture`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Refine mapping/validation for `BTS capture ( Alarms )` to `VisitPhoto.PhotoCategory`.
  Priority: **Low**
- Excel column name: `3G capture`
  Sheet name: `alarms capture`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Refine mapping/validation for `3G capture` to `VisitPhoto.PhotoCategory`.
  Priority: **Low**
- Excel column name: `MW capture`
  Sheet name: `alarms capture`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Refine mapping/validation for `MW capture` to `VisitPhoto.PhotoCategory`.
  Priority: **Low**
- Excel column name: `File`
  Sheet name: `Audit matrix SQI`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Add new field/entity for `File`.
  Priority: **Low**
- Excel column name: `Network Audit Checklist (applicable on entire radio sites)`
  Sheet name: `Audit matrix SQI`
  File name: `GH-DE  Checklist.xlsx`
  Recommended domain change: Refine mapping/validation for `Network Audit Checklist (applicable on entire radio sites)` to `VisitChecklist.Category/ItemName/Status`.
  Priority: **High**
- Excel column name: `Short Code`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Short Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `Name`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Name`.
  Priority: **Low**
- Excel column name: `Site Host`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Site Host`.
  Priority: **Low**
- Excel column name: `Host Code`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Host Code` to `SiteSharing.HostOperator`.
  Priority: **Low**
- Excel column name: `Site Guests`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Guests` to `SiteSharing.GuestOperators`.
  Priority: **Low**
- Excel column name: `Topology`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Topology` to `SiteTowerInfo.TowerType`.
  Priority: **Low**
- Excel column name: `TX Enclosure`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX Enclosure`.
  Priority: **Low**
- Excel column name: `Sharing Count Radio Antenna`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Sharing Count Radio Antenna`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 1`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 1`.
  Priority: **Low**
- Excel column name: `Radio HBA 1`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 1`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 2`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 2`.
  Priority: **Low**
- Excel column name: `Radio HBA 2`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 2`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 3`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 3`.
  Priority: **Low**
- Excel column name: `Radio HBA 3`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 3`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 4`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 4`.
  Priority: **Low**
- Excel column name: `Radio HBA 4`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 4`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 5`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 5`.
  Priority: **Low**
- Excel column name: `Radio HBA 5`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 5`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 6`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 6`.
  Priority: **Low**
- Excel column name: `Radio HBA 6`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 6`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 7`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 7`.
  Priority: **Low**
- Excel column name: `Radio HBA 7`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 7`.
  Priority: **Low**
- Excel column name: `Radio Azimuth 8`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio Azimuth 8`.
  Priority: **Low**
- Excel column name: `Radio HBA 8`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Radio HBA 8`.
  Priority: **Low**
- Excel column name: `Sharing Count Tx Antenna`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Sharing Count Tx Antenna`.
  Priority: **Low**
- Excel column name: `TX Azimuth 1`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 1`.
  Priority: **Low**
- Excel column name: `TX HBA 1`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 1`.
  Priority: **Low**
- Excel column name: `TX Azimuth 2`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 2`.
  Priority: **Low**
- Excel column name: `TX HBA 2`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 2`.
  Priority: **Low**
- Excel column name: `TX Azimuth 3`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 3`.
  Priority: **Low**
- Excel column name: `TX HBA 3`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 3`.
  Priority: **Low**
- Excel column name: `TX Azimuth 4`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 4`.
  Priority: **Low**
- Excel column name: `TX HBA 4`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 4`.
  Priority: **Low**
- Excel column name: `TX Azimuth 5`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 5`.
  Priority: **Low**
- Excel column name: `TX HBA 5`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 5`.
  Priority: **Low**
- Excel column name: `TX Azimuth 6`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 6`.
  Priority: **Low**
- Excel column name: `TX HBA 6`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 6`.
  Priority: **Low**
- Excel column name: `TX Azimuth 7`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 7`.
  Priority: **Low**
- Excel column name: `TX HBA 7`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 7`.
  Priority: **Low**
- Excel column name: `TX Azimuth 8`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 8`.
  Priority: **Low**
- Excel column name: `TX HBA 8`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 8`.
  Priority: **Low**
- Excel column name: `TX Azimuth 9`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth 9`.
  Priority: **Low**
- Excel column name: `TX HBA 9`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA 9`.
  Priority: **Low**
- Excel column name: `Remarks`
  Sheet name: `Site Sharing Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Remarks` to `VisitChecklist/VisitIssue.Notes/Description`.
  Priority: **Low**
- Excel column name: `Short Code`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Short Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `Local Site Name`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Local Site Name` to `Site.Name/OMCName`.
  Priority: **Low**
- Excel column name: `Directions Site Name`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Directions Site Name` to `MWLink.OppositeSite`.
  Priority: **Low**
- Excel column name: `Directions Site Code`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Directions Site Code` to `MWLink.OppositeSite`.
  Priority: **High**
- Excel column name: `Mast No.`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Mast No.` to `SiteTowerInfo.NumberOfMasts`.
  Priority: **Low**
- Excel column name: `TX HBA`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX HBA`.
  Priority: **Low**
- Excel column name: `TX Azimuth`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TX Azimuth`.
  Priority: **Low**
- Excel column name: `Antenna Diameter [m]`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Antenna Diameter [m]` to `MWLink.DishSizeCM`.
  Priority: **Low**
- Excel column name: `Side Arm Height [m]`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Side Arm Height [m]`.
  Priority: **Low**
- Excel column name: `Side Arm Diameter`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Side Arm Diameter`.
  Priority: **Low**
- Excel column name: `IP Address`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Link Type`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Link Type` to `SiteTransmission.TransmissionType`.
  Priority: **Low**
- Excel column name: `Link Model`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Link Model` to `MWLink.ODUType`.
  Priority: **Low**
- Excel column name: `Band`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Band`.
  Priority: **Low**
- Excel column name: `Configuration`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Configuration`.
  Priority: **Low**
- Excel column name: `Modulation`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Capacity [Mb/s]`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Tx Frequency [KHz]`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Rx Frequency [KHz]`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Tx Power [dBm]`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **High**
- Excel column name: `Rx Power [dBm]`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **High**
- Excel column name: `ODU Model`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `ODU S/N`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Opposite site ODU S/N`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Antenna Reference`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Antenna Reference`.
  Priority: **Low**
- Excel column name: `Polarization`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Remarks`
  Sheet name: `Site TX Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Remarks` to `VisitChecklist/VisitIssue.Notes/Description`.
  Priority: **Low**
- Excel column name: `Short Code`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Short Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `Name`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Name`.
  Priority: **Low**
- Excel column name: `Date Of Visit`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Date Of Visit` to `Visit.ScheduledDate/CheckInTime`.
  Priority: **High**
- Excel column name: `Sector Technology`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Sector Technology` to `SectorInfo.Technology`.
  Priority: **Low**
- Excel column name: `Sector number`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Sector number`.
  Priority: **Low**
- Excel column name: `Site Topology`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Topology` to `SiteTowerInfo.TowerType`.
  Priority: **Low**
- Excel column name: `Building Height (m)`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Building Height (m)` to `SiteTowerInfo.Height`.
  Priority: **Low**
- Excel column name: `Support Height (m)`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Support Height (m)` to `SiteTowerInfo.Height`.
  Priority: **Low**
- Excel column name: `Support ID`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Support ID` to `SiteTowerInfo.NumberOfMasts`.
  Priority: **Low**
- Excel column name: `Azimuth`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Azimuth`.
  Priority: **Low**
- Excel column name: `Antenna Type`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Antenna Type`.
  Priority: **Low**
- Excel column name: `Antenna Sharing`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Antenna Sharing` to `SiteSharing.IsShared`.
  Priority: **Low**
- Excel column name: `RRU/RF/RRH Solution`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `RRU/RF/RRH Solution`.
  Priority: **Medium**
- Excel column name: `Combiner/ Diplexer ID`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Combiner/ Diplexer ID`.
  Priority: **Low**
- Excel column name: `Mast Head Amplifier`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Mast Head Amplifier`.
  Priority: **Low**
- Excel column name: `Bias-T`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Bias-T`.
  Priority: **Low**
- Excel column name: `HBA(m)`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `HBA(m)`.
  Priority: **Low**
- Excel column name: `Bracket Tilt`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Bracket Tilt`.
  Priority: **Low**
- Excel column name: `Elect Tilt`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Elect Tilt`.
  Priority: **Low**
- Excel column name: `Feeder Size / Cable Type`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Feeder Size / Cable Type`.
  Priority: **Low**
- Excel column name: `Feeder Length`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Feeder Length`.
  Priority: **Low**
- Excel column name: `Remarks`
  Sheet name: `Site Radio Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Remarks` to `VisitChecklist/VisitIssue.Notes/Description`.
  Priority: **Low**
- Excel column name: `#`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `#`.
  Priority: **Low**
- Excel column name: `Site Code`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `BSC Code`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `BSC Code`.
  Priority: **Low**
- Excel column name: `BSC Name`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `BSC Name`.
  Priority: **Low**
- Excel column name: `OEG Site Name`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `OEG Site Name` to `Site.Name/OMCName`.
  Priority: **Low**
- Excel column name: `TE Name`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `TE Name`.
  Priority: **Low**
- Excel column name: `No . Of locations`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `No . Of locations`.
  Priority: **Low**
- Excel column name: `Site Type`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Type` to `Site.SiteType`.
  Priority: **High**
- Excel column name: `A/C Units`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `A/C Units` to `SiteCoolingSystem.ACUnits/ACUnitsCount`.
  Priority: **Low**
- Excel column name: `A/C Capacity`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Rectifier No.`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Rectifier No.` to `SitePowerSystem.RectifierModulesCount`.
  Priority: **High**
- Excel column name: `Routers`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Routers` to `SiteTransmission.HasALURouter`.
  Priority: **Low**
- Excel column name: `GPS`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `GPS` to `SiteTransmission.HasGPS`.
  Priority: **Low**
- Excel column name: `ADM`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `ADM` to `SiteTransmission.HasADM`.
  Priority: **Low**
- Excel column name: `Modem`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Modem`.
  Priority: **Low**
- Excel column name: `TX Indoor Qty.`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `TX Indoor Qty.` to `SiteTransmission.LinksCount`.
  Priority: **Low**
- Excel column name: `TX Indoor Type`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `TX Indoor Type` to `MWLink.ODUType`.
  Priority: **Low**
- Excel column name: `BSC Qty`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `BSC Qty`.
  Priority: **Low**
- Excel column name: `BSC Type`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `BSC Type`.
  Priority: **Low**
- Excel column name: `BTS Vendor`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `BTS Vendor`.
  Priority: **Low**
- Excel column name: `BTS QTY`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `BTS QTY`.
  Priority: **Low**
- Excel column name: `2G RF  Modules`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `3G RF Modules`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `4G RF Modules`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Cabinet Type (Y/N) Y=Rectifier in Cabinet`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add cabinet metadata fields to `SitePowerSystem`.
  Priority: **High**
- Excel column name: `Battery Type`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Battery Type` to `SitePowerSystem.BatteryType/BatteryVoltage/BatteryAmpereHour`.
  Priority: **High**
- Excel column name: `Battery Strings Qty`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Battery Strings Qty` to `SitePowerSystem.BatteryStrings/BatteriesPerString`.
  Priority: **High**
- Excel column name: `Rectifier Type`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Rectifier Type` to `SitePowerSystem.RectifierBrand`.
  Priority: **High**
- Excel column name: `No of Modules`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `TE Current C.B Rate`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `TE Current C.B Rate` to `SitePowerSystem.PowerMeterRate`.
  Priority: **Low**
- Excel column name: `Date Of Visit`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Date Of Visit` to `Visit.ScheduledDate/CheckInTime`.
  Priority: **High**
- Excel column name: `Contact Person`
  Sheet name: `Power Data`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Contact Person`.
  Priority: **Low**
- Excel column name: `ShortCode`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `ShortCode` to `Site.SiteCode`.
  Priority: **Low**
- Excel column name: `Site OMC Name`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Site OMC Name`.
  Priority: **Low**
- Excel column name: `Site Code`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `BSC Name`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `BSC Name`.
  Priority: **Low**
- Excel column name: `Subcontractor`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Subcontractor`.
  Priority: **Low**
- Excel column name: `Maintenance Area`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Maintenance Area`.
  Priority: **Low**
- Excel column name: `Region`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Region`.
  Priority: **High**
- Excel column name: `Subregion`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Subregion`.
  Priority: **High**
- Excel column name: `Subcontractor Office`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Subcontractor Office` to `Office.Code/Name`.
  Priority: **Low**
- Excel column name: `Announcement Date`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Announcement Date` to `Site.AnnouncementDate`.
  Priority: **High**
- Excel column name: `On / Off  Air`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `On / Off  Air` to `Site.Status`.
  Priority: **Low**
- Excel column name: `Site Coordinates X, Y`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Coordinates X, Y` to `Site.Coordinates`.
  Priority: **Low**
- Excel column name: `Address Detailes`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Address Detailes` to `Site.Address`.
  Priority: **Low**
- Excel column name: `Sharing`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Sharing` to `SiteSharing.IsShared`.
  Priority: **Low**
- Excel column name: `Tower`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Tower` to `SiteTowerInfo.TowerType`.
  Priority: **High**
- Excel column name: `Radio Equipment Data`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Radio Equipment Data` to `SiteTransmission.TransmissionType`.
  Priority: **Low**
- Excel column name: `DC Power`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **High**
- Excel column name: `AC Power`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **High**
- Excel column name: `Cooling System`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Cooling System` to `SiteCoolingSystem.ACUnits/ACUnitsCount`.
  Priority: **Low**
- Excel column name: `Fire Panel`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Fire Panel`.
  Priority: **Low**
- Excel column name: `ZTE Remote Monitoring System`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add remote monitoring capability fields under Site.
  Priority: **Low**
- Excel column name: `General Data`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `General Data`.
  Priority: **Low**
- Excel column name: `Status`
  Sheet name: `Site Assets Data Count`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Status` to `Site.Status`.
  Priority: **High**
- Excel column name: `Site code`
  Sheet name: `RF Status`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `Site code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `site name`
  Sheet name: `RF Status`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `site name`.
  Priority: **Low**
- Excel column name: `total RF In site`
  Sheet name: `RF Status`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `total RF In site`.
  Priority: **Medium**
- Excel column name: `RF on tower Number`
  Sheet name: `RF Status`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `RF on tower Number`.
  Priority: **High**
- Excel column name: `RF on ground Number`
  Sheet name: `RF Status`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `RF on ground Number`.
  Priority: **Medium**
- Excel column name: `Number RF sector carry`
  Sheet name: `RF Status`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `Number RF sector carry`.
  Priority: **Medium**
- Excel column name: `band for RF on tower`
  Sheet name: `RF Status`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `band for RF on tower`.
  Priority: **High**
- Excel column name: `band for RF on ground`
  Sheet name: `RF Status`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Add new field/entity for `band for RF on ground`.
  Priority: **Medium**
- Excel column name: `comment`
  Sheet name: `RF Status`
  File name: `GH-DE Data Collection.xlsx`
  Recommended domain change: Refine mapping/validation for `comment` to `VisitChecklist/VisitIssue.Notes/Description`.
  Priority: **Low**
- Excel column name: `#`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Add new field/entity for `#`.
  Priority: **Low**
- Excel column name: `Site Code`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Code` to `Site.SiteCode`.
  Priority: **High**
- Excel column name: `BSC Code`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Add new field/entity for `BSC Code`.
  Priority: **Low**
- Excel column name: `BSC Name`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Add new field/entity for `BSC Name`.
  Priority: **Low**
- Excel column name: `OEG Site Name`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `OEG Site Name` to `Site.Name/OMCName`.
  Priority: **Low**
- Excel column name: `TE Name`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Add new field/entity for `TE Name`.
  Priority: **Low**
- Excel column name: `No . Of locations`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Add new field/entity for `No . Of locations`.
  Priority: **Low**
- Excel column name: `Site Type`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `Site Type` to `Site.SiteType`.
  Priority: **High**
- Excel column name: `A/C Units`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `A/C Units` to `SiteCoolingSystem.ACUnits/ACUnitsCount`.
  Priority: **Low**
- Excel column name: `A/C Capacity`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Rectifier No.`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `Rectifier No.` to `SitePowerSystem.RectifierModulesCount`.
  Priority: **High**
- Excel column name: `Routers`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `Routers` to `SiteTransmission.HasALURouter`.
  Priority: **Low**
- Excel column name: `GPS`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `GPS` to `SiteTransmission.HasGPS`.
  Priority: **Low**
- Excel column name: `ADM`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `ADM` to `SiteTransmission.HasADM`.
  Priority: **Low**
- Excel column name: `Modem`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Add new field/entity for `Modem`.
  Priority: **Low**
- Excel column name: `TX Indoor Qty.`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `TX Indoor Qty.` to `SiteTransmission.LinksCount`.
  Priority: **Low**
- Excel column name: `TX Indoor Type`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `TX Indoor Type` to `MWLink.ODUType`.
  Priority: **Low**
- Excel column name: `BSC Qty`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Add new field/entity for `BSC Qty`.
  Priority: **Low**
- Excel column name: `BSC Type`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Add new field/entity for `BSC Type`.
  Priority: **Low**
- Excel column name: `BTS Vendor`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Add new field/entity for `BTS Vendor`.
  Priority: **Low**
- Excel column name: `BTS QTY`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Add new field/entity for `BTS QTY`.
  Priority: **Low**
- Excel column name: `2G RF  Modules`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `3G RF Modules`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `4G RF Modules`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `Cabinet Type (Y/N) Y=Rectifier in Cabinet`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Add cabinet metadata fields to `SitePowerSystem`.
  Priority: **High**
- Excel column name: `Battery Type`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `Battery Type` to `SitePowerSystem.BatteryType/BatteryVoltage/BatteryAmpereHour`.
  Priority: **High**
- Excel column name: `Battery Strings Qty`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `Battery Strings Qty` to `SitePowerSystem.BatteryStrings/BatteriesPerString`.
  Priority: **High**
- Excel column name: `Rectifier Type`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `Rectifier Type` to `SitePowerSystem.RectifierBrand`.
  Priority: **High**
- Excel column name: `No of Modules`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Extend `MWLink` / `SiteTransmission` with RF/link telemetry fields.
  Priority: **Medium**
- Excel column name: `TE Current C.B Rate`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `TE Current C.B Rate` to `SitePowerSystem.PowerMeterRate`.
  Priority: **Low**
- Excel column name: `Date Of Visit`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Refine mapping/validation for `Date Of Visit` to `Visit.ScheduledDate/CheckInTime`.
  Priority: **High**
- Excel column name: `Contact Person`
  Sheet name: `Power Data`
  File name: `Power Data.xlsx`
  Recommended domain change: Add new field/entity for `Contact Person`.
  Priority: **Low**

## Section 3: Partial Mappings

- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `ShortCode`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `Site Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `Subcontractor Office`
  - What is mapped: `Office.Code/Name`
  - What is missing: Workbook uses aliases/names; system import requires normalized office codes.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `Announcement Date`
  - What is mapped: `Site.AnnouncementDate`
  - What is missing: Excel serial/text date parsing and UTC normalization required.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `On / Off  Air`
  - What is mapped: `Site.Status`
  - What is missing: Template lifecycle vocabulary is richer than SiteStatus enum.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `Site Coordinates X, Y`
  - What is mapped: `Site.Coordinates`
  - What is missing: Coordinates are in one combined cell; domain expects latitude and longitude separately.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `Address Detailes`
  - What is mapped: `Site.Address`
  - What is missing: Structured address components are not split in template.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `Sharing`
  - What is mapped: `SiteSharing.IsShared`
  - What is missing: Contains shared state plus operator semantics.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `Tower`
  - What is mapped: `SiteTowerInfo.TowerType`
  - What is missing: Values combine site topology and tower type text.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `Radio Equipment Data`
  - What is mapped: `SiteTransmission.TransmissionType`
  - What is missing: Mixed domain meanings in source column.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `DC Power`
  - What is mapped: `SitePowerSystem.BatteryType`
  - What is missing: Source stores brand/value strings not matching enum.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `AC Power`
  - What is mapped: `SitePowerSystem.PowerConfiguration`
  - What is missing: Requires translation from business shorthand (EC, ET EC, etc.).
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `Cooling System`
  - What is mapped: `SiteCoolingSystem.ACUnits/ACUnitsCount`
  - What is missing: Values are free text and composite, not normalized numeric/unit records.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Assets Data Count` / `Status`
  - What is mapped: `Site.Status`
  - What is missing: Template lifecycle vocabulary is richer than SiteStatus enum.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `Site Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `OEG Site Name`
  - What is mapped: `Site.Name/OMCName`
  - What is missing: Semantics vary by sheet (canonical site vs local TX context).
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `Site Type`
  - What is mapped: `Site.SiteType`
  - What is missing: Template values (BTS/BSC/Green Field/Roof Top) do not map 1:1 to enum.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `A/C Units`
  - What is mapped: `SiteCoolingSystem.ACUnits/ACUnitsCount`
  - What is missing: Values are free text and composite, not normalized numeric/unit records.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `A/C Capacity`
  - What is mapped: `SiteCoolingSystem.ACUnits/ACUnitsCount`
  - What is missing: Values are free text and composite, not normalized numeric/unit records.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `Rectifier No.`
  - What is mapped: `SitePowerSystem.RectifierModulesCount`
  - What is missing: May represent rectifier units vs module counts depending on sheet.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `Routers`
  - What is mapped: `SiteTransmission.HasALURouter`
  - What is missing: Only boolean modeled; source can include counts/types.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `GPS`
  - What is mapped: `SiteTransmission.HasGPS`
  - What is missing: Text values require normalization.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `ADM`
  - What is mapped: `SiteTransmission.HasADM`
  - What is missing: Text values require normalization.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `TX Indoor Qty.`
  - What is mapped: `SiteTransmission.LinksCount`
  - What is missing: Indoor equipment count is not the same as MW links count.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `TX Indoor Type`
  - What is mapped: `MWLink.ODUType`
  - What is missing: Per-link modeling required; source is often site-level summary.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `Battery Type`
  - What is mapped: `SitePowerSystem.BatteryType/BatteryVoltage/BatteryAmpereHour`
  - What is missing: Template combines multiple dimensions in one field and uses non-enum brands.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `Battery Strings Qty`
  - What is mapped: `SitePowerSystem.BatteryStrings/BatteriesPerString`
  - What is missing: Rows often contain comma-separated or textual values.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `Rectifier Type`
  - What is mapped: `SitePowerSystem.RectifierBrand`
  - What is missing: Template brands include values not represented in enum.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `No of Modules`
  - What is mapped: `SitePowerSystem.RectifierModulesCount`
  - What is missing: May represent rectifier units vs module counts depending on sheet.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `TE Current C.B Rate`
  - What is mapped: `SitePowerSystem.PowerMeterRate`
  - What is missing: Needs explicit semantic mapping rule.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Power Data` / `Date Of Visit`
  - What is mapped: `Visit.ScheduledDate/CheckInTime`
  - What is missing: No direct linkage between these template rows and Visit aggregate IDs.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Radio Data` / `Short Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Radio Data` / `Date Of Visit`
  - What is mapped: `Visit.ScheduledDate/CheckInTime`
  - What is missing: No direct linkage between these template rows and Visit aggregate IDs.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Radio Data` / `Sector Technology`
  - What is mapped: `SectorInfo.Technology`
  - What is missing: Band-level values (GU900, D1800) not supported by current enum.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Radio Data` / `Site Topology`
  - What is mapped: `SiteTowerInfo.TowerType`
  - What is missing: Values combine site topology and tower type text.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Radio Data` / `Building Height (m)`
  - What is mapped: `SiteTowerInfo.Height`
  - What is missing: Template has multiple height dimensions but model has one main height.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Radio Data` / `Support Height (m)`
  - What is mapped: `SiteTowerInfo.Height`
  - What is missing: Template has multiple height dimensions but model has one main height.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Radio Data` / `Support ID`
  - What is mapped: `SiteTowerInfo.NumberOfMasts`
  - What is missing: Support identifier and mast count are different concepts.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Radio Data` / `Antenna Sharing`
  - What is mapped: `SiteSharing.IsShared`
  - What is missing: Per-antenna sharing context is not modeled.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Radio Data` / `Remarks`
  - What is mapped: `VisitChecklist/VisitIssue.Notes/Description`
  - What is missing: Context target (site, tx, power, checklist) is ambiguous.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site TX Data` / `Short Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site TX Data` / `Local Site Name`
  - What is mapped: `Site.Name/OMCName`
  - What is missing: Semantics vary by sheet (canonical site vs local TX context).
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site TX Data` / `Directions Site Name`
  - What is mapped: `MWLink.OppositeSite`
  - What is missing: Remote site relation is flat text, not structured reference.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site TX Data` / `Directions Site Code`
  - What is mapped: `MWLink.OppositeSite`
  - What is missing: Remote site relation is flat text, not structured reference.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site TX Data` / `Mast No.`
  - What is mapped: `SiteTowerInfo.NumberOfMasts`
  - What is missing: Mast index per TX link is not same as aggregate mast count.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site TX Data` / `Antenna Diameter [m]`
  - What is mapped: `MWLink.DishSizeCM`
  - What is missing: Unit mismatch (meters vs cm). 
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site TX Data` / `Link Type`
  - What is mapped: `SiteTransmission.TransmissionType`
  - What is missing: PDH/other values not covered by enum.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site TX Data` / `Link Model`
  - What is mapped: `MWLink.ODUType`
  - What is missing: Per-link modeling required; source is often site-level summary.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site TX Data` / `ODU Model`
  - What is mapped: `MWLink.ODUType`
  - What is missing: Per-link modeling required; source is often site-level summary.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site TX Data` / `Remarks`
  - What is mapped: `VisitChecklist/VisitIssue.Notes/Description`
  - What is missing: Context target (site, tx, power, checklist) is ambiguous.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Sharing Data` / `Short Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Sharing Data` / `Host Code`
  - What is mapped: `SiteSharing.HostOperator`
  - What is missing: Host site code is not modeled separately from host operator.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Sharing Data` / `Site Guests`
  - What is mapped: `SiteSharing.GuestOperators`
  - What is missing: Requires split/parsing for multi-operator values.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Sharing Data` / `Topology`
  - What is mapped: `SiteTowerInfo.TowerType`
  - What is missing: Values combine site topology and tower type text.
- `Data collection from 1-1-2023 till 31-12-2023.xlsx` / `Site Sharing Data` / `Remarks`
  - What is mapped: `VisitChecklist/VisitIssue.Notes/Description`
  - What is missing: Context target (site, tx, power, checklist) is ambiguous.
- `Delta Sites (1).xlsx` / `Sheet2` / `Short Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `Delta Sites (1).xlsx` / `Sheet2` / `Office`
  - What is mapped: `Office.Code/Name`
  - What is missing: Workbook uses aliases/names; system import requires normalized office codes.
- `Delta Sites (1).xlsx` / `Sheet2` / `Rectifier Brand`
  - What is mapped: `SitePowerSystem.RectifierBrand`
  - What is missing: Template brands include values not represented in enum.
- `Delta Sites (1).xlsx` / `Sheet2` / `Battery Type/Volt/AH`
  - What is mapped: `SitePowerSystem.BatteryType/BatteryVoltage/BatteryAmpereHour`
  - What is missing: Template combines multiple dimensions in one field and uses non-enum brands.
- `Delta Sites (1).xlsx` / `Sheet2` / `No of String`
  - What is mapped: `SitePowerSystem.BatteryStrings/BatteriesPerString`
  - What is missing: Rows often contain comma-separated or textual values.
- `Delta Sites (1).xlsx` / `Sheet2` / `Announcement Date`
  - What is mapped: `Site.AnnouncementDate`
  - What is missing: Excel serial/text date parsing and UTC normalization required.
- `Delta Sites (1).xlsx` / `Sheet1` / `Short Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `Delta Sites (1).xlsx` / `Sheet1` / `SC Office`
  - What is mapped: `Office.Code/Name`
  - What is missing: Workbook uses aliases/names; system import requires normalized office codes.
- `GH-BDT_BDT.xlsx` / `Summary ` / `Short Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `GH-BDT_BDT.xlsx` / `Summary ` / `On Air Date`
  - What is mapped: `Site.AnnouncementDate`
  - What is missing: Excel serial/text date parsing and UTC normalization required.
- `GH-BDT_BDT.xlsx` / `Summary ` / `Power Source`
  - What is mapped: `SitePowerSystem.PowerConfiguration`
  - What is missing: Requires translation from business shorthand (EC, ET EC, etc.).
- `GH-BDT_BDT.xlsx` / `Summary ` / `AC1 Type`
  - What is mapped: `SiteCoolingSystem.ACUnits/ACUnitsCount`
  - What is missing: Values are free text and composite, not normalized numeric/unit records.
- `GH-BDT_BDT.xlsx` / `Summary ` / `AC HP`
  - What is mapped: `SiteCoolingSystem.ACUnits/ACUnitsCount`
  - What is missing: Values are free text and composite, not normalized numeric/unit records.
- `GH-BDT_BDT.xlsx` / `Summary ` / `AC2 Type`
  - What is mapped: `SiteCoolingSystem.ACUnits/ACUnitsCount`
  - What is missing: Values are free text and composite, not normalized numeric/unit records.
- `GH-BDT_BDT.xlsx` / `Summary ` / `AC HP3`
  - What is mapped: `SiteCoolingSystem.ACUnits/ACUnitsCount`
  - What is missing: Values are free text and composite, not normalized numeric/unit records.
- `GH-BDT_BDT.xlsx` / `Summary ` / `Orange office`
  - What is mapped: `Office.Code/Name`
  - What is missing: Workbook uses aliases/names; system import requires normalized office codes.
- `GH-BDT_BDT.xlsx` / `Summary ` / `Office`
  - What is mapped: `Office.Code/Name`
  - What is missing: Workbook uses aliases/names; system import requires normalized office codes.
- `GH-BDT_BDT.xlsx` / `Summary ` / `Rectifier Brand`
  - What is mapped: `SitePowerSystem.RectifierBrand`
  - What is missing: Template brands include values not represented in enum.
- `GH-BDT_BDT.xlsx` / `Summary ` / `Battery Brand`
  - What is mapped: `SitePowerSystem.BatteryType/BatteryVoltage/BatteryAmpereHour`
  - What is missing: Template combines multiple dimensions in one field and uses non-enum brands.
- `GH-BDT_BDT.xlsx` / `Summary ` / `No of String`
  - What is mapped: `SitePowerSystem.BatteryStrings/BatteriesPerString`
  - What is missing: Rows often contain comma-separated or textual values.
- `GH-BDT_BDT.xlsx` / `Summary ` / `No of Batteries`
  - What is mapped: `SitePowerSystem.BatteryStrings/BatteriesPerString`
  - What is missing: Rows often contain comma-separated or textual values.
- `GH-BDT_BDT.xlsx` / `Summary ` / `Comment`
  - What is mapped: `VisitChecklist/VisitIssue.Notes/Description`
  - What is missing: Context target (site, tx, power, checklist) is ambiguous.
- `GH-DE  Checklist.xlsx` / `Panorama` / `Shelter panorama`
  - What is mapped: `VisitPhoto.PhotoCategory`
  - What is missing: Evidence media columns exist but metadata/timestamps and import semantics are missing.
- `GH-DE  Checklist.xlsx` / `Tower Panorama` / `Tower panorama`
  - What is mapped: `VisitPhoto.PhotoCategory`
  - What is missing: Evidence media columns exist but metadata/timestamps and import semantics are missing.
- `GH-DE  Checklist.xlsx` / `alarms capture` / `BTS capture ( Alarms )`
  - What is mapped: `VisitPhoto.PhotoCategory`
  - What is missing: Evidence media columns exist but metadata/timestamps and import semantics are missing.
- `GH-DE  Checklist.xlsx` / `alarms capture` / `3G capture`
  - What is mapped: `VisitPhoto.PhotoCategory`
  - What is missing: Evidence media columns exist but metadata/timestamps and import semantics are missing.
- `GH-DE  Checklist.xlsx` / `alarms capture` / `MW capture`
  - What is mapped: `VisitPhoto.PhotoCategory`
  - What is missing: Evidence media columns exist but metadata/timestamps and import semantics are missing.
- `GH-DE  Checklist.xlsx` / `Audit matrix SQI` / `Network Audit Checklist (applicable on entire radio sites)`
  - What is mapped: `VisitChecklist.Category/ItemName/Status`
  - What is missing: Template metadata is not normalized as checklist items.
- `GH-DE Data Collection.xlsx` / `Site Sharing Data` / `Short Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `GH-DE Data Collection.xlsx` / `Site Sharing Data` / `Host Code`
  - What is mapped: `SiteSharing.HostOperator`
  - What is missing: Host site code is not modeled separately from host operator.
- `GH-DE Data Collection.xlsx` / `Site Sharing Data` / `Site Guests`
  - What is mapped: `SiteSharing.GuestOperators`
  - What is missing: Requires split/parsing for multi-operator values.
- `GH-DE Data Collection.xlsx` / `Site Sharing Data` / `Topology`
  - What is mapped: `SiteTowerInfo.TowerType`
  - What is missing: Values combine site topology and tower type text.
- `GH-DE Data Collection.xlsx` / `Site Sharing Data` / `Remarks`
  - What is mapped: `VisitChecklist/VisitIssue.Notes/Description`
  - What is missing: Context target (site, tx, power, checklist) is ambiguous.
- `GH-DE Data Collection.xlsx` / `Site TX Data` / `Short Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `GH-DE Data Collection.xlsx` / `Site TX Data` / `Local Site Name`
  - What is mapped: `Site.Name/OMCName`
  - What is missing: Semantics vary by sheet (canonical site vs local TX context).
- `GH-DE Data Collection.xlsx` / `Site TX Data` / `Directions Site Name`
  - What is mapped: `MWLink.OppositeSite`
  - What is missing: Remote site relation is flat text, not structured reference.
- `GH-DE Data Collection.xlsx` / `Site TX Data` / `Directions Site Code`
  - What is mapped: `MWLink.OppositeSite`
  - What is missing: Remote site relation is flat text, not structured reference.
- `GH-DE Data Collection.xlsx` / `Site TX Data` / `Mast No.`
  - What is mapped: `SiteTowerInfo.NumberOfMasts`
  - What is missing: Mast index per TX link is not same as aggregate mast count.
- `GH-DE Data Collection.xlsx` / `Site TX Data` / `Antenna Diameter [m]`
  - What is mapped: `MWLink.DishSizeCM`
  - What is missing: Unit mismatch (meters vs cm). 
- `GH-DE Data Collection.xlsx` / `Site TX Data` / `Link Type`
  - What is mapped: `SiteTransmission.TransmissionType`
  - What is missing: PDH/other values not covered by enum.
- `GH-DE Data Collection.xlsx` / `Site TX Data` / `Link Model`
  - What is mapped: `MWLink.ODUType`
  - What is missing: Per-link modeling required; source is often site-level summary.
- `GH-DE Data Collection.xlsx` / `Site TX Data` / `ODU Model`
  - What is mapped: `MWLink.ODUType`
  - What is missing: Per-link modeling required; source is often site-level summary.
- `GH-DE Data Collection.xlsx` / `Site TX Data` / `Remarks`
  - What is mapped: `VisitChecklist/VisitIssue.Notes/Description`
  - What is missing: Context target (site, tx, power, checklist) is ambiguous.
- `GH-DE Data Collection.xlsx` / `Site Radio Data` / `Short Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `GH-DE Data Collection.xlsx` / `Site Radio Data` / `Date Of Visit`
  - What is mapped: `Visit.ScheduledDate/CheckInTime`
  - What is missing: No direct linkage between these template rows and Visit aggregate IDs.
- `GH-DE Data Collection.xlsx` / `Site Radio Data` / `Sector Technology`
  - What is mapped: `SectorInfo.Technology`
  - What is missing: Band-level values (GU900, D1800) not supported by current enum.
- `GH-DE Data Collection.xlsx` / `Site Radio Data` / `Site Topology`
  - What is mapped: `SiteTowerInfo.TowerType`
  - What is missing: Values combine site topology and tower type text.
- `GH-DE Data Collection.xlsx` / `Site Radio Data` / `Building Height (m)`
  - What is mapped: `SiteTowerInfo.Height`
  - What is missing: Template has multiple height dimensions but model has one main height.
- `GH-DE Data Collection.xlsx` / `Site Radio Data` / `Support Height (m)`
  - What is mapped: `SiteTowerInfo.Height`
  - What is missing: Template has multiple height dimensions but model has one main height.
- `GH-DE Data Collection.xlsx` / `Site Radio Data` / `Support ID`
  - What is mapped: `SiteTowerInfo.NumberOfMasts`
  - What is missing: Support identifier and mast count are different concepts.
- `GH-DE Data Collection.xlsx` / `Site Radio Data` / `Antenna Sharing`
  - What is mapped: `SiteSharing.IsShared`
  - What is missing: Per-antenna sharing context is not modeled.
- `GH-DE Data Collection.xlsx` / `Site Radio Data` / `Remarks`
  - What is mapped: `VisitChecklist/VisitIssue.Notes/Description`
  - What is missing: Context target (site, tx, power, checklist) is ambiguous.
- `GH-DE Data Collection.xlsx` / `Power Data` / `Site Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `GH-DE Data Collection.xlsx` / `Power Data` / `OEG Site Name`
  - What is mapped: `Site.Name/OMCName`
  - What is missing: Semantics vary by sheet (canonical site vs local TX context).
- `GH-DE Data Collection.xlsx` / `Power Data` / `Site Type`
  - What is mapped: `Site.SiteType`
  - What is missing: Template values (BTS/BSC/Green Field/Roof Top) do not map 1:1 to enum.
- `GH-DE Data Collection.xlsx` / `Power Data` / `A/C Units`
  - What is mapped: `SiteCoolingSystem.ACUnits/ACUnitsCount`
  - What is missing: Values are free text and composite, not normalized numeric/unit records.
- `GH-DE Data Collection.xlsx` / `Power Data` / `A/C Capacity`
  - What is mapped: `SiteCoolingSystem.ACUnits/ACUnitsCount`
  - What is missing: Values are free text and composite, not normalized numeric/unit records.
- `GH-DE Data Collection.xlsx` / `Power Data` / `Rectifier No.`
  - What is mapped: `SitePowerSystem.RectifierModulesCount`
  - What is missing: May represent rectifier units vs module counts depending on sheet.
- `GH-DE Data Collection.xlsx` / `Power Data` / `Routers`
  - What is mapped: `SiteTransmission.HasALURouter`
  - What is missing: Only boolean modeled; source can include counts/types.
- `GH-DE Data Collection.xlsx` / `Power Data` / `GPS`
  - What is mapped: `SiteTransmission.HasGPS`
  - What is missing: Text values require normalization.
- `GH-DE Data Collection.xlsx` / `Power Data` / `ADM`
  - What is mapped: `SiteTransmission.HasADM`
  - What is missing: Text values require normalization.
- `GH-DE Data Collection.xlsx` / `Power Data` / `TX Indoor Qty.`
  - What is mapped: `SiteTransmission.LinksCount`
  - What is missing: Indoor equipment count is not the same as MW links count.
- `GH-DE Data Collection.xlsx` / `Power Data` / `TX Indoor Type`
  - What is mapped: `MWLink.ODUType`
  - What is missing: Per-link modeling required; source is often site-level summary.
- `GH-DE Data Collection.xlsx` / `Power Data` / `Battery Type`
  - What is mapped: `SitePowerSystem.BatteryType/BatteryVoltage/BatteryAmpereHour`
  - What is missing: Template combines multiple dimensions in one field and uses non-enum brands.
- `GH-DE Data Collection.xlsx` / `Power Data` / `Battery Strings Qty`
  - What is mapped: `SitePowerSystem.BatteryStrings/BatteriesPerString`
  - What is missing: Rows often contain comma-separated or textual values.
- `GH-DE Data Collection.xlsx` / `Power Data` / `Rectifier Type`
  - What is mapped: `SitePowerSystem.RectifierBrand`
  - What is missing: Template brands include values not represented in enum.
- `GH-DE Data Collection.xlsx` / `Power Data` / `No of Modules`
  - What is mapped: `SitePowerSystem.RectifierModulesCount`
  - What is missing: May represent rectifier units vs module counts depending on sheet.
- `GH-DE Data Collection.xlsx` / `Power Data` / `TE Current C.B Rate`
  - What is mapped: `SitePowerSystem.PowerMeterRate`
  - What is missing: Needs explicit semantic mapping rule.
- `GH-DE Data Collection.xlsx` / `Power Data` / `Date Of Visit`
  - What is mapped: `Visit.ScheduledDate/CheckInTime`
  - What is missing: No direct linkage between these template rows and Visit aggregate IDs.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `ShortCode`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `Site Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `Subcontractor Office`
  - What is mapped: `Office.Code/Name`
  - What is missing: Workbook uses aliases/names; system import requires normalized office codes.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `Announcement Date`
  - What is mapped: `Site.AnnouncementDate`
  - What is missing: Excel serial/text date parsing and UTC normalization required.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `On / Off  Air`
  - What is mapped: `Site.Status`
  - What is missing: Template lifecycle vocabulary is richer than SiteStatus enum.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `Site Coordinates X, Y`
  - What is mapped: `Site.Coordinates`
  - What is missing: Coordinates are in one combined cell; domain expects latitude and longitude separately.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `Address Detailes`
  - What is mapped: `Site.Address`
  - What is missing: Structured address components are not split in template.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `Sharing`
  - What is mapped: `SiteSharing.IsShared`
  - What is missing: Contains shared state plus operator semantics.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `Tower`
  - What is mapped: `SiteTowerInfo.TowerType`
  - What is missing: Values combine site topology and tower type text.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `Radio Equipment Data`
  - What is mapped: `SiteTransmission.TransmissionType`
  - What is missing: Mixed domain meanings in source column.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `DC Power`
  - What is mapped: `SitePowerSystem.BatteryType`
  - What is missing: Source stores brand/value strings not matching enum.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `AC Power`
  - What is mapped: `SitePowerSystem.PowerConfiguration`
  - What is missing: Requires translation from business shorthand (EC, ET EC, etc.).
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `Cooling System`
  - What is mapped: `SiteCoolingSystem.ACUnits/ACUnitsCount`
  - What is missing: Values are free text and composite, not normalized numeric/unit records.
- `GH-DE Data Collection.xlsx` / `Site Assets Data Count` / `Status`
  - What is mapped: `Site.Status`
  - What is missing: Template lifecycle vocabulary is richer than SiteStatus enum.
- `GH-DE Data Collection.xlsx` / `RF Status` / `Site code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `GH-DE Data Collection.xlsx` / `RF Status` / `comment`
  - What is mapped: `VisitChecklist/VisitIssue.Notes/Description`
  - What is missing: Context target (site, tx, power, checklist) is ambiguous.
- `Power Data.xlsx` / `Power Data` / `Site Code`
  - What is mapped: `Site.SiteCode`
  - What is missing: Template code formats (e.g., 3564DE, C1_3564DE_PH13) do not match current SiteCode parser.
- `Power Data.xlsx` / `Power Data` / `OEG Site Name`
  - What is mapped: `Site.Name/OMCName`
  - What is missing: Semantics vary by sheet (canonical site vs local TX context).
- `Power Data.xlsx` / `Power Data` / `Site Type`
  - What is mapped: `Site.SiteType`
  - What is missing: Template values (BTS/BSC/Green Field/Roof Top) do not map 1:1 to enum.
- `Power Data.xlsx` / `Power Data` / `A/C Units`
  - What is mapped: `SiteCoolingSystem.ACUnits/ACUnitsCount`
  - What is missing: Values are free text and composite, not normalized numeric/unit records.
- `Power Data.xlsx` / `Power Data` / `A/C Capacity`
  - What is mapped: `SiteCoolingSystem.ACUnits/ACUnitsCount`
  - What is missing: Values are free text and composite, not normalized numeric/unit records.
- `Power Data.xlsx` / `Power Data` / `Rectifier No.`
  - What is mapped: `SitePowerSystem.RectifierModulesCount`
  - What is missing: May represent rectifier units vs module counts depending on sheet.
- `Power Data.xlsx` / `Power Data` / `Routers`
  - What is mapped: `SiteTransmission.HasALURouter`
  - What is missing: Only boolean modeled; source can include counts/types.
- `Power Data.xlsx` / `Power Data` / `GPS`
  - What is mapped: `SiteTransmission.HasGPS`
  - What is missing: Text values require normalization.
- `Power Data.xlsx` / `Power Data` / `ADM`
  - What is mapped: `SiteTransmission.HasADM`
  - What is missing: Text values require normalization.
- `Power Data.xlsx` / `Power Data` / `TX Indoor Qty.`
  - What is mapped: `SiteTransmission.LinksCount`
  - What is missing: Indoor equipment count is not the same as MW links count.
- `Power Data.xlsx` / `Power Data` / `TX Indoor Type`
  - What is mapped: `MWLink.ODUType`
  - What is missing: Per-link modeling required; source is often site-level summary.
- `Power Data.xlsx` / `Power Data` / `Battery Type`
  - What is mapped: `SitePowerSystem.BatteryType/BatteryVoltage/BatteryAmpereHour`
  - What is missing: Template combines multiple dimensions in one field and uses non-enum brands.
- `Power Data.xlsx` / `Power Data` / `Battery Strings Qty`
  - What is mapped: `SitePowerSystem.BatteryStrings/BatteriesPerString`
  - What is missing: Rows often contain comma-separated or textual values.
- `Power Data.xlsx` / `Power Data` / `Rectifier Type`
  - What is mapped: `SitePowerSystem.RectifierBrand`
  - What is missing: Template brands include values not represented in enum.
- `Power Data.xlsx` / `Power Data` / `No of Modules`
  - What is mapped: `SitePowerSystem.RectifierModulesCount`
  - What is missing: May represent rectifier units vs module counts depending on sheet.
- `Power Data.xlsx` / `Power Data` / `TE Current C.B Rate`
  - What is mapped: `SitePowerSystem.PowerMeterRate`
  - What is missing: Needs explicit semantic mapping rule.
- `Power Data.xlsx` / `Power Data` / `Date Of Visit`
  - What is mapped: `Visit.ScheduledDate/CheckInTime`
  - What is missing: No direct linkage between these template rows and Visit aggregate IDs.

## Section 4: Recommended Domain Changes

- Entity name: `Site`  
  Field to add: `LegacyShortCode` + alternate parser for `ShortCode/Site Code` formats  
  Data type: `string` + normalization service  
  Priority: **High**
- Entity name: `Site`  
  Field to add: `OperationalZone` and `ExternalContextNotes`  
  Data type: `string`  
  Priority: **Medium**
- Entity name: `SitePowerSystem`  
  Field to add: `CabinetType`, `IsCabinetized`, `BatteryHealthStatus`, `PlvdLlvdValue`, `ChargingCurrentLimit`  
  Data type: `string`, `bool`, `string`, `decimal?`, `decimal?`  
  Priority: **High**
- Entity name: `MWLink`  
  Field to add: `ManagementIp`, `TxFrequencyKHz`, `RxFrequencyKHz`, `TxPowerDbm`, `RxPowerDbm`, `CapacityMbps`, `Modulation`, `Polarization`, `Configuration`, `OduSerialNumber`, `OppositeOduSerialNumber`, `AntennaReference`, `Azimuth`, `HbaMeters`  
  Data type: numeric/string fields  
  Priority: **High**
- Entity name: `SectorInfo` / `SiteRadioEquipment`  
  Field to add: band-aware sector technology + feeder specs  
  Data type: enum + `string/int`  
  Priority: **High**
- Entity name: `SiteSharing`  
  Field to add: shared antenna counts and per-antenna azimuth/HBA child collection  
  Data type: `int` + collection  
  Priority: **Medium**
- Entity name: `VisitIssue`  
  Field to add: `TargetDate`  
  Data type: `DateTime?` (UTC)  
  Priority: **High**
- Entity name: `VisitPhoto`  
  Field to add: `CapturedAtUtc` explicit column mapping  
  Data type: `DateTime`  
  Priority: **Medium**
- Entity name: `BatteryDischargeTest` (new aggregate)  
  Field to add: start/end V/A, discharge minutes, stop reason, repeat reason, CAP ref  
  Data type: numeric + text + date  
  Priority: **High**
- Entity name: `UnusedAsset` (new)  
  Field to add: site-linked unused reserves/assets register  
  Data type: new entity  
  Priority: **Low**

## Section 5: Import Readiness

- **Data collection from 1-1-2023 till 31-12-2023.xlsx**
  - Can it be imported using existing `ImportSiteDataCommand`? **Partial** (only core site master columns from `Site Assets Data Count`).
  - Additional import commands needed: `ImportSiteAssetsCommand`, `ImportPowerDataCommand`, `ImportSiteRadioDataCommand`, `ImportSiteTxDataCommand`, `ImportSiteSharingDataCommand`.
  - Validation rules to add: legacy site code parser, enum normalization for tower/power/site type, coordinate split parser, Excel serial date conversion to UTC.
- **Delta Sites (1).xlsx**
  - Can it be imported using existing `ImportSiteDataCommand`? **Partial**.
  - Additional import commands needed: `ImportDeltaBatteryStatusCommand`.
  - Validation rules to add: office name->code normalization, battery type decomposition (type/volt/AH), nodal degree support.
- **GH-BDT_BDT.xlsx**
  - Can it be imported using existing `ImportSiteDataCommand`? **No** (most sheets have no row-1 headers and represent BDT process forms).
  - Additional import commands needed: `ImportBatteryDischargeTestCommand`.
  - Validation rules to add: typed numeric parsing for start/end readings and discharge timing, mandatory site/date context.
- **GH-DE  Checklist.xlsx**
  - Can it be imported using existing `ImportSiteDataCommand`? **No** (form/photo template workbook; mostly non-tabular row-1 layout).
  - Additional import commands needed: `ImportChecklistTemplateCommand`, `ImportPanoramaEvidenceCommand`, `ImportAlarmCaptureCommand`.
  - Validation rules to add: checklist item normalization, evidence metadata completeness (category, timestamp, source).
- **GH-DE Data Collection.xlsx**
  - Can it be imported using existing `ImportSiteDataCommand`? **Partial** (site master subset only).
  - Additional import commands needed: same as 2023 data collection + `ImportRfStatusCommand`.
  - Validation rules to add: cross-sheet key integrity by short/site code, per-sheet schema-specific coercion.
- **Power Data.xlsx**
  - Can it be imported using existing `ImportSiteDataCommand`? **Partial**.
  - Additional import commands needed: `ImportPowerInventoryCommand`.
  - Validation rules to add: `SiteType` normalization (`BTS/BSC/Green Field`), boolean normalization (`GPS/ADM/Routers`), numeric coercion for module/string counts.
