![image](https://github.com/user-attachments/assets/50678fc8-6251-455a-94b2-14763c889ddf)
First page of UploadThings. At this page the system will be automatically create necessary database and table in MSSQL, MariaDB, and Postgres. The test at DatabaseCheckerTest.cs.

![image](https://github.com/user-attachments/assets/c93cfb2c-a7be-4877-ac82-dd40e1fbb154)
User Index page. At this page all users will be listed in this page. The test at UserRepositoryTest.cs.

![image](https://github.com/user-attachments/assets/6afcc254-f2dc-47dd-a23e-8a384671e082)
User Create page. At this page we could add new user. The test at UserRepositoryTest.cs.

![image](https://github.com/user-attachments/assets/f91ead67-8e00-4ca5-a218-5a4b32131fd4)
Product Index page. At this page all product will be listed in this page. The test at ProductRepositoryTest.cs.

![image](https://github.com/user-attachments/assets/c1a294cc-7161-44d3-a7db-af8bf264500e)
Product Create page. At this page we could add new product. The test at ProductRepositoryTest.cs.

![image](https://github.com/user-attachments/assets/b561cbfb-9057-4092-af6a-9473010c2b5f)
Transaction Index page. At this page all transaction will be listed in this page. The test at TransactionRepositoryTest.cs.

![image](https://github.com/user-attachments/assets/edcb7f7d-d54e-4f25-a9c7-faf77a40eb48)
Transaction Create page. At this page we could add new transaction. The test at TransactionRepositoryTest.cs.

![High Level Design](https://github.com/user-attachments/assets/e751d458-927c-449c-bf34-662e7854a521)
High level design architectur. I try to use some of the design pattern like :
1. Factories Method to provides an interface for creating objects in a superclass, but allows subclasses to alter 
the type of objects that will be created.
2. Singleton Method to let us ensure that a class has only one instance, while providing a global access point to this instance.
3. Abstract Factories Method to let us produce families of related objects without specifying their concrete classes.
I use this architecture so I could reuse a lot of recurring function without writing it again, use it globally, and open to extention which means it scalable.








