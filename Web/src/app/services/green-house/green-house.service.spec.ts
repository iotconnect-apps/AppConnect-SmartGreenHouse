import { TestBed } from '@angular/core/testing';

import { GreenHouseService } from './green-house.service';

describe('GreenHouseService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: GreenHouseService = TestBed.get(GreenHouseService);
    expect(service).toBeTruthy();
  });
});
