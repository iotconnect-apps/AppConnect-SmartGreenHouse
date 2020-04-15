import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GatewayAddComponent } from './gateway-add.component';

describe('GatewayAddComponent', () => {
  let component: GatewayAddComponent;
  let fixture: ComponentFixture<GatewayAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GatewayAddComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GatewayAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
