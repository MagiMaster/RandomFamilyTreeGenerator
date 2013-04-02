using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FamilyGen {
    public class Person {
        static Random rnd = new Random();

        public bool isMale, isMarried, partial;
        public string firstName, lastName, maidenName;
        public string hair, eyes;
        public string hobby;
        public int age, birth, death;

        public Person mother, father, spouse;
        public List<Person> children;

        #region Helper Functions

        public static string RandomMaleName() { return "MALE"; }
        public static string RandomFemaleName() { return "FEMALE"; }
        public static string RandomLastName() { return "LAST"; }
        public static string RandomHobby(int year) { return "HOBBY"; }

        public static string RandomHair() {
            int r = rnd.Next(4);
            switch (r) {
                case 0: return "Blonde";
                case 1: return "Black";
                case 2: return "Brown";
                case 3: return "Red";
            }
            return "ERROR";
        }

        public static string RandomEyes() {
            int r = rnd.Next(4);
            switch (r) {
                case 0: return "Blue";
                case 1: return "Green";
                case 2: return "Brown";
                case 3: return "Black";
            }
            return "ERROR";
        }

        #endregion

        #region Factories

        private Person(string gender = null)
        {
            if(gender == null || gender.Length == 0)
                isMale = rnd.Next() % 2 == 0;
            else if (gender[0] == 'm' || gender[0] == 'M')
                isMale = true;
            else if(gender[0] == 'f' || gender[0] == 'F')
                isMale = false;
            else
                isMale = rnd.Next() % 2 == 0;

            if (isMale)
                firstName = RandomMaleName();
            else 
                firstName = RandomFemaleName();

            children = new List<Person>();

            partial = true;
        }

        // Generate a person from no info
        public static Person GeneratePerson() {
            Person p = new Person();

            int ageRange = (int)(50.0 * Math.Exp(0.00035 * MainForm.mainForm.year));
            p.age = Math.Abs(rnd.Next(1, ageRange) + rnd.Next(1, ageRange / 20) - rnd.Next(1, ageRange));

            if (p.age > 14) {
                double c = p.age / 12.0;
                c *= c;
                c = 1.0 - 1.0 / (1.0 + c);
                c *= c * c;

                p.isMarried = rnd.NextDouble() < c;

                if (p.isMarried) {
                    int n = 0;
                    for (int i = 0; i < 6; ++i)
                        n += rnd.Next(0, 2);
                    n /= 2;

                    for (int i = 0; i < n; ++i)
                        p.children.Add(null);
                }

            } else
                p.isMarried = false;

            p.lastName = RandomLastName();
            p.maidenName = (p.isMale || !p.isMarried ? p.lastName : RandomLastName());
            p.hair = RandomHair();
            p.eyes = RandomEyes();

            p.birth = MainForm.mainForm.year - p.age;

            p.hobby = RandomHobby(p.birth);

            do {
                p.death = p.birth + ageRange + rnd.Next(1, ageRange / 10);
            } while (p.death <= MainForm.mainForm.year);

            return p;
        }

        public static Person GenerateFather(Person p) {
            Person f = new Person("M");

            int ageDiff = 14 + Math.Abs(rnd.Next(0, 30) - rnd.Next(0, 30));
            f.birth = p.birth - ageDiff;
            f.age = MainForm.mainForm.year - f.birth;

            int ageRange = (int)(50.0 * Math.Exp(0.00035 * f.birth));
            f.death = f.birth + ageRange + rnd.Next(1, ageRange / 10);

            f.lastName = p.maidenName;
            f.maidenName = p.maidenName;

            f.hair = RandomHair();
            f.eyes = RandomEyes();

            f.hobby = RandomHobby(f.birth);

            int n;
            do {
                n = 0;
                for (int i = 0; i < 6; ++i)
                    n += rnd.Next(0, 2);
                n /= 2;
            } while (n == 0);

            for (int i = 0; i < n; ++i)
                f.children.Add(null);

            f.children[rnd.Next(0, n)] = p;

            f.isMarried = true;

            return f;
        }

        public static Person GenerateMother(Person p) {
            Person m = new Person("F");

            int ageDiff = 14 + Math.Abs(rnd.Next(0, 30) - rnd.Next(0, 30));
            m.birth = p.birth - ageDiff;
            m.age = MainForm.mainForm.year - m.birth;

            int ageRange = (int)(50.0 * Math.Exp(0.00035 * m.birth));
            m.death = m.birth + ageRange + rnd.Next(1, ageRange / 10);

            m.lastName = p.maidenName;
            m.maidenName = RandomLastName();

            m.hobby = RandomHobby(m.birth);

            m.hair = RandomHair();
            m.eyes = RandomEyes();

            m.isMarried = true;

            return m;
        }

        public static Person GenerateChild(Person p) {
            Person c = new Person();

            do {
                int ageDiff = 14 + Math.Abs(rnd.Next(0, 30) - rnd.Next(0, 30));
                c.birth = p.birth + ageDiff;
                c.age = MainForm.mainForm.year - c.birth;
            } while (c.age < 0);

            int ageRange = (int)(50.0 * Math.Exp(0.00035 * c.birth));
            c.death = c.birth + ageRange + rnd.Next(1, ageRange / 10);

            c.maidenName = p.lastName;

            c.hobby = RandomHobby(c.birth);

            if(p.isMale) {
                c.father = p;
                c.mother = p.spouse;
            } else {
                c.father = p.spouse;
                c.mother = p;
            }

            c.hair = rnd.NextDouble() < 0.5 ? c.mother.hair : c.father.hair;
            c.eyes = rnd.NextDouble() < 0.5 ? c.mother.eyes : c.father.eyes;

            if (c.age > 14) {
                double k = c.age / 12.0;
                k *= k;
                k = 1.0 - 1.0 / (1.0 + k);
                k *= k * k;

                c.isMarried = rnd.NextDouble() < k;

                if (c.isMarried) {
                    if(!c.isMale)
                        c.lastName = RandomLastName();

                    int n = 0;
                    for (int i = 0; i < 6; ++i)
                        n += rnd.Next(0, 2);
                    n /= 2;

                    for (int i = 0; i < n; ++i)
                        c.children.Add(null);
                }
            } else
                c.isMarried = false;

            return c;
        }

        public static Person GenerateSpouse(Person p) {
            Person s = new Person();
            s.isMale = !p.isMale;


            //...


            s.children = p.children;
            foreach (Person c in s.children)
                if (c != null) {
                    if (s.isMale)
                        c.father = s;
                    else
                        c.mother = s;
                }

            return s;
        }

        #endregion

        #region Member Functions

        public string fullName {
            get {
                return firstName + " " + lastName;
            }
        }

        public void FillData(PersonPanel pp)
        {
            if (!partial)
                return;

            if (mother == null && father == null) {
                mother = GenerateMother(this);
                MainForm.mainForm.AddPerson(mother, pp.Location.X + 72, pp.Location.Y - 64);

                father = GenerateFather(this);
                MainForm.mainForm.AddPerson(father, pp.Location.X - 72, pp.Location.Y - 64);

                if (rnd.NextDouble() < 0.5)
                    mother.eyes = eyes;
                else
                    father.eyes = eyes;

                if (rnd.NextDouble() < 0.5)
                    mother.hair = hair;
                else
                    father.hair = hair;

                mother.children = father.children;
                mother.spouse = father;
                father.spouse = mother;
            } else if (father == null) {
                father = GenerateFather(this);
                mother.spouse = father;
                mother.children = father.children;

                if (mother.hair != hair)
                    father.hair = hair;
                if (mother.eyes != eyes)
                    father.eyes = eyes;

                MainForm.mainForm.AddPerson(father, pp.Location.X - 72, pp.Location.Y - 64);
            } else if (mother == null) {
                mother = GenerateMother(this);
                father.spouse = mother;
                mother.children = father.children;

                if (father.hair != hair)
                    mother.hair = hair;
                if (father.eyes != eyes)
                    mother.eyes = eyes;

                MainForm.mainForm.AddPerson(mother, pp.Location.X + 72, pp.Location.Y - 64);
            }

            if (isMarried && spouse == null) {
                spouse = GenerateSpouse(this);
                MainForm.mainForm.AddPerson(spouse, pp.Location.X + (isMale ? 216 : -216), pp.Location.Y);
            }

            for (int i = 0; i < children.Count; ++i)
                if (children[i] == null) {
                    children[i] = GenerateChild(this);
                    MainForm.mainForm.AddPerson(children[i], pp.Location.X + i * 144 - (children.Count - 1) * 72, pp.Location.Y + 64);
                }

            partial = false;
        }

        #endregion
    }
}
