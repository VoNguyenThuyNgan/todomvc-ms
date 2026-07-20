import { Routes } from '@angular/router';
import { TodosComponent } from './todos-component/todos-component';

export const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'all'
    },
    {
        path: 'all',
        component: TodosComponent
    },
    {
        path: 'active',
        component: TodosComponent
    },
    {
        path: 'completed',
        component: TodosComponent
    },
    {
        path: '**',
        redirectTo: 'all'
    }
];
