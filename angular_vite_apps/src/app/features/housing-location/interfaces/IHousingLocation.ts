interface HousingLocation {
  id: number;
  name: string;
  city: string;
  state: string;
  photo: string;
  availableUnits: number;
  wifi: boolean;
  laundry: boolean;
}

interface GetAllHousingLocation {
  housingLocationData: HousingLocation[];
}

// Grouped export at the end of the file
export type { HousingLocation, GetAllHousingLocation };
