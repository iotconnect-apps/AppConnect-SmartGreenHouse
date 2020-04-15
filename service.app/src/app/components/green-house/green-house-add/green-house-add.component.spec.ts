import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GreenHouseAddComponent } from './green-house-add.component';

describe('GreenHouseAddComponent', () => {
  let component: GreenHouseAddComponent;
  let fixture: ComponentFixture<GreenHouseAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GreenHouseAddComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GreenHouseAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
