import pandas as pd

import warnings
warnings.filterwarnings("ignore", category=UserWarning)

personer_df = pd.read_excel("G:\\Min enhet\\Styrelsens dokument\\Träning\\Gradering.xlsx","Personregister")          # Download personregister from https://kansli.sportadmin.se/personregister/personregister
personer_df.rename(columns={
    "Personnummer":"personnumber",
    "Kön":"sex",
    "Förnamn":"firstname",
    "Efternamn":"lastname",
    "c/o":"co",
    "Adress":"address",
    "Postnummer":"postalcode",
    "Stad":"city",
    "Land":"country",
    "Mobiltelefon":"mobile",
    "Telefon hem":"phone_home",
    "Telefon jobb":"phone_work",
    "E-post":"email",
    "Målsman 1":"guardian1",
    "Relation":"guardian1_relation",
    "E-post.1":"guardian1_email",
    "Telefon":"guardian1_phone",
    "Målsman 2":"guardian2",
    "Relation.1":"guardian2_relation",
    "E-post.2":"guardian2_email",
    "Telefon.1":"guardian2_phone",
    "Skapad":"created",
    "Uppdaterad":"updated",
    "Grupprekommendation":"grouprecomendation",
    "Övrigt":"other",
    "MedlemsNr":"membernumber",
    "StartÅr":"started",
    "Allergi":"allergy"
  })


narvaro_df = pd.read_excel("G:\\Min enhet\\Styrelsens dokument\\Träning\\Gradering.xlsx","Närvaro")

df.to_json("Personregister.json", orient="records", force_ascii=False, indent=2)



