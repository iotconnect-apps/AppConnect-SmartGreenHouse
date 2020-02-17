import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CropsListComponent } from './crops-list.component';

describe('CropsListComponent', () => {
  let component: CropsListComponent;
  let fixture: ComponentFixture<CropsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CropsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CropsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
