import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GreenHouseDetailsComponent } from './green-house-details.component';

describe('GreenHouseDetailsComponent', () => {
  let component: GreenHouseDetailsComponent;
  let fixture: ComponentFixture<GreenHouseDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GreenHouseDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GreenHouseDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
