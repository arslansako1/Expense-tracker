import { useState } from 'react';
import reactLogo from './assets/react.svg';
import viteLogo from './assets/vite.svg';
import heroImg from './assets/hero.png';
import './App.css';
import Intro from './Components/Intro';
import Login from './Components/Login';
import { Navigate, Route, Routes } from 'react-router-dom';
import ForgetPassword from './Components/ForgetPassword';
import ResetPassword from './Components/ResetPassword';
import Signup from './Components/Signup';
import MainPage from './Components/Dashboard/MainPage';
import Accounts from './Components/Dashboard/AccountsPage/Accounts';
import Categories from './Components/Dashboard/CategoriesPage/Categories';
import Budgets from './Components/Dashboard/BudgetPage/Budgets';
import Transactions from './Components/Dashboard/TransactionsPage/Transactions';
import Transfers from './Components/Dashboard/TransfersPage/Transfers';
import AutoPayments from './Components/Dashboard/AutoPaymentsPage/AutoPayments';
import ImportBankStatement from './Components/Dashboard/ImportBankPage/ImportBankStatement';
import Nofications from './Components/Dashboard/NoficationsPage/Nofications';
import Profile from './Components/Dashboard/ProfilePage/Profile';
import ViewAccTransactions from './Components/Dashboard/AccountsPage/ViewAccTransactions';
import ViewAccTransfers from './Components/Dashboard/AccountsPage/ViewAccTransfers';
import Reports from './Components/Dashboard/ReportsPage/Reports';

function App() {

  return(
    <Routes>
     <Route path='/Login' element={<Login/>}/>
     <Route path='/Signup' element={<Signup/>}/>
     <Route path='/ForgetPassword' element={<ForgetPassword/>}/>
     <Route path='/ResetPassword' element={<ResetPassword/>}/>
     <Route path='/MainPage' element={<MainPage/>}/>
     <Route path='/Accounts' element={<Accounts/>}/>
     <Route path='/Categories' element={<Categories/>}/>
     <Route path='/Budgets' element={<Budgets/>}/>
     <Route path='/Transactions' element={<Transactions/>}/>
     <Route path='/Transfers' element={<Transfers/>}/>
     <Route path='/AutoPayments' element={<AutoPayments/>}/>
     <Route path='/ImportBankStatement' element={<ImportBankStatement/>}/>
     <Route path='/Reports' element={<Reports/>}/>
     <Route path='/Nofications' element={<Nofications/>}/>
     <Route path='/ViewAccTransactions' element={<ViewAccTransactions/>}/>
     <Route path='/ViewAccTransfers' element={<ViewAccTransfers/>}/>
     <Route path='/Profile' element={<Profile/>}/>

     <Route path="/Intro" element={<Intro/>}/>

      <Route path="/" element={<Navigate to="/Intro"/>}/>

    </Routes>
  
  )
}

export default App;