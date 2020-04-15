import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GreenHouseListComponent } from './green-house-list.component';

describe('GreenHouseListComponent', () => {
  let component: GreenHouseListComponent;
  let fixture: ComponentFixture<GreenHouseListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GreenHouseListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GreenHouseListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
