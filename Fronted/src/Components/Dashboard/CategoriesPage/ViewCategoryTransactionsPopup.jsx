import { useEffect, useRef, useState } from "react";
import { API_ENDPOINTS } from "../../../Configration/Urls";
import "/src/Css/Dashboard/CategoryPage/ViewCategoryTransactions.css";


export default function ViewCategoryTransactionsPopup({allTransactions, category, onClose}){
    const dialogRef = useRef(null);

    const categoryTransactions = allTransactions.filter(
        t => t.categoryId === category.id
    );
    
    const totalIncomeAmount = categoryTransactions
        .filter(t => t.type === "Income")
        .reduce((sum, t) => sum + t.amount, 0);
    
    const totalExpenseAmount = categoryTransactions
        .filter(t => t.type === "Expense")
        .reduce((sum, t) => sum + t.amount, 0);
    
    const totalTransactions = categoryTransactions.length;

    useEffect(() => {
      dialogRef.current.showModal();
      
   
    }, [])

    return(
    <dialog className="viewCategoryTransactions-dialog" ref={dialogRef}>
      <button type="button" className="create-close-btn" onClick={() => onClose()}>✕</button>

         <h2 className="allTransactionName">All {category.name} transactions:</h2>
         <h4 className="incomeExpense">Total income: {totalIncomeAmount} Total expense: {totalExpenseAmount}</h4>
                <h3 className="totalTransactions">totalTransactions: {totalTransactions}</h3>
                <h2 className="amount-above">Amount</h2>
                <h2 className="type-above">Type</h2>
                <h2 className="description-above">Description</h2>
            {categoryTransactions.map((transaction) => (
              <div key={transaction.id}>
                <h3 className="amount-bottom">{transaction.amount}</h3>
                <h3 className={`type-bottom ${transaction.type.toLowerCase()}`}>{transaction.type}</h3>
                <h3 className="description-bottom">{transaction.description}</h3>
             
              </div>
          ))}     

    </dialog>
  );
    
}