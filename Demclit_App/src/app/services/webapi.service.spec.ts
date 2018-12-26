import { TestBed, inject } from '@angular/core/testing';

import { WebapiService } from './webapi.service';

describe('WebapiService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [WebapiService]
    });
  });

  it('should be created', inject([WebapiService], (service: WebapiService) => {
    expect(service).toBeTruthy();
  }));
});
